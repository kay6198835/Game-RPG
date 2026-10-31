using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace QA.Automation.Editor
{
    // Entry point of the weekly QA run. Runs the STATIC group in Edit Mode, then builds each QA scene and
    // plays it; each scene's QAHarness writes partial-<group>.json and leaves Play Mode, which triggers the
    // next group. At the end the partials are merged into results.json + unity-report.md.
    //
    // Command line (Editor closed; no -batchmode: Play Mode needs the normal player loop; no -quit):
    //   Unity.exe -projectPath <project> -executeMethod QA.Automation.Editor.QARunner.RunFromCommandLine
    //     -qaOut <dir> [-qaGroups STATIC,SYSTEMS,VITALS,ENEMY,PLAYER] [-qaMax P1] [-qaDeep ABL,VIT|ALL] [-qaSeed N]
    //     -logFile <log>
    // Exit code: 0 = no FAIL, 1 = at least one FAIL, 2 = the runner itself failed.
    [InitializeOnLoad]
    public static class QARunner
    {
        private const string QueueKey = "QA.Automation.Queue";
        private const string CurrentKey = "QA.Automation.Current";
        public static readonly string[] AllGroups = { "STATIC", "SYSTEMS", "VITALS", "ENEMY", "PLAYER" };

        static QARunner()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        [MenuItem("Tools/QA/Automation/Run P0 + P1")]
        public static void RunRegressionMenu() => StartMenu(new QARunConfig
        {
            outputDir = System.IO.Path.Combine(QARunConfig.ProjectRoot, "qa-automation", "reports", $"manual-{DateTime.Now:yyyyMMdd-HHmm}"),
        });

        [MenuItem("Tools/QA/Automation/Run Everything (P0-P2)")]
        public static void RunEverythingMenu() => StartMenu(QARunConfig.Everything());

        [MenuItem("Tools/QA/Automation/Build QA Scenes")]
        public static void BuildScenesMenu()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            foreach (string group in QASceneBuilder.PlayModeGroups) QASceneBuilder.Build(group);
        }

        private static void StartMenu(QARunConfig config)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            SessionState.SetBool(QAHarness.MenuRunKey, true);
            Begin(config, AllGroups);
        }

        public static void RunFromCommandLine()
        {
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                QARunConfig config = new()
                {
                    outputDir = Arg(args, "-qaOut") ?? System.IO.Path.Combine(QARunConfig.ProjectRoot, "qa-automation", "reports", $"cli-{DateTime.Now:yyyyMMdd-HHmm}"),
                    maxPriority = Arg(args, "-qaMax") ?? nameof(QAPriority.P1),
                };
                string deep = Arg(args, "-qaDeep") ?? "";
                config.deepAll = deep.Equals("ALL", StringComparison.OrdinalIgnoreCase);
                if (!config.deepAll) config.deepSystems = deep.Split(',').Select(s => s.Trim().ToUpperInvariant()).Where(s => s.Length > 0).ToList();
                if (int.TryParse(Arg(args, "-qaSeed"), out int seed)) config.seed = seed;
                string[] groups = (Arg(args, "-qaGroups") ?? string.Join(",", AllGroups)).Split(',').Select(s => s.Trim().ToUpperInvariant()).Where(s => s.Length > 0).ToArray();

                SessionState.SetBool(QAHarness.CliFlagKey, true);
                Begin(config, groups);
            }
            catch (Exception e)
            {
                Debug.LogError($"[QA] runner failed to start: {e}");
                EditorApplication.Exit(2);
            }
        }

        private static string Arg(string[] args, string name)
        {
            int i = Array.IndexOf(args, name);
            return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
        }

        private static void Begin(QARunConfig config, string[] groups)
        {
            Directory.CreateDirectory(config.outputDir);
            foreach (string old in Directory.GetFiles(config.outputDir, "partial-*.json")) File.Delete(old);
            config.Save();
            SessionState.SetString(QueueKey, string.Join(",", groups));
            Debug.Log($"[QA] run start: groups {string.Join(",", groups)}, max {config.maxPriority}, deep {(config.deepAll ? "ALL" : string.Join(",", config.deepSystems))}, out {config.outputDir}");
            Advance();
        }

        private static bool Active => SessionState.GetBool(QAHarness.CliFlagKey, false) || SessionState.GetBool(QAHarness.MenuRunKey, false);

        private static void OnPlayModeChanged(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.EnteredEditMode || !Active) return;
            // Deferred: starting the next Play Mode from inside this callback is not reliable.
            EditorApplication.delayCall += () =>
            {
                try
                {
                    MarkCrashedGroup();
                    Advance();
                }
                catch (Exception e)
                {
                    Abort(e);
                }
            };
        }

        private static void Advance()
        {
            QARunConfig config = QARunConfig.Load();
            List<string> queue = SessionState.GetString(QueueKey, "").Split(',').Where(s => s.Length > 0).ToList();
            while (queue.Count > 0)
            {
                string group = queue[0];
                queue.RemoveAt(0);
                SessionState.SetString(QueueKey, string.Join(",", queue));
                SessionState.SetString(CurrentKey, group);

                if (group == "STATIC")
                {
                    QAResultFile file = QAStaticChecks.Run(config);
                    File.WriteAllText(System.IO.Path.Combine(config.outputDir, "partial-STATIC.json"), JsonUtility.ToJson(file, true));
                    continue;
                }
                if (!QASceneBuilder.PlayModeGroups.Contains(group))
                {
                    Debug.LogWarning($"[QA] unknown group {group}, skipped");
                    continue;
                }

                string scene = QASceneBuilder.Build(group);
                EditorSceneManager.OpenScene(scene);
                EditorApplication.isPlaying = true;
                return;
            }

            SessionState.SetString(CurrentKey, "");
            int code = Finish(config);
            bool cli = SessionState.GetBool(QAHarness.CliFlagKey, false);
            SessionState.SetBool(QAHarness.CliFlagKey, false);
            SessionState.SetBool(QAHarness.MenuRunKey, false);
            if (cli)
            {
                Debug.Log($"[QA] run finished, exiting with code {code}");
                EditorApplication.Exit(code);
            }
        }

        // A group whose Play Mode ended without a partial file crashed or was stopped by hand.
        private static void MarkCrashedGroup()
        {
            string group = SessionState.GetString(CurrentKey, "");
            if (string.IsNullOrEmpty(group) || group == "STATIC") return;
            QARunConfig config = QARunConfig.Load();
            string path = System.IO.Path.Combine(config.outputDir, $"partial-{group}.json");
            if (File.Exists(path)) return;
            QAResultFile file = new() { group = group, scene = QASceneBuilder.ScenePath(group), started = "", finished = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
            file.cases.Add(new QACaseResult
            {
                id = $"QA-RUNNER.{group}", title = $"Group {group} finished Play Mode and wrote its results", system = "RUNNER",
                priority = nameof(QAPriority.P0), group = group, verdict = nameof(QAVerdict.FAIL),
                notes = new List<string> { "FAIL: Play Mode ended without partial results (crash, exception in suite setup, or stopped by hand) — read the Unity log" },
            });
            File.WriteAllText(path, JsonUtility.ToJson(file, true));
        }

        private static int Finish(QARunConfig config)
        {
            List<QAResultFile> files = Directory.GetFiles(config.outputDir, "partial-*.json")
                .Select(f => JsonUtility.FromJson<QAResultFile>(File.ReadAllText(f))).ToList();
            List<QACaseResult> all = files.SelectMany(f => f.cases).ToList();

            QARunSummary summary = new()
            {
                finished = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                unity = Application.unityVersion,
                maxPriority = config.maxPriority,
                deep = config.deepAll ? "ALL" : string.Join(",", config.deepSystems),
                seed = config.seed,
                groups = files.Select(f => f.group).ToList(),
                cases = all,
            };
            File.WriteAllText(System.IO.Path.Combine(config.outputDir, "results.json"), JsonUtility.ToJson(summary, true));
            File.WriteAllText(System.IO.Path.Combine(config.outputDir, "unity-report.md"), Markdown(summary));
            foreach (string partial in Directory.GetFiles(config.outputDir, "partial-*.json")) File.Delete(partial);

            int fails = all.Count(c => c.verdict == nameof(QAVerdict.FAIL));
            Debug.Log($"[QA] SUMMARY total={all.Count} pass={Count(all, QAVerdict.PASS)} fail={fails} skip={Count(all, QAVerdict.SKIP)} observed={Count(all, QAVerdict.OBSERVED)} out={config.outputDir}");
            return fails > 0 ? 1 : 0;
        }

        private static int Count(List<QACaseResult> all, QAVerdict v) => all.Count(c => c.verdict == v.ToString());

        private static string Markdown(QARunSummary s)
        {
            StringBuilder sb = new();
            sb.AppendLine("# QA automation — raw Unity results");
            sb.AppendLine();
            sb.AppendLine($"- Finished {s.finished}, Unity {s.unity}, priority ≤ {s.maxPriority}, deep P2: {(string.IsNullOrEmpty(s.deep) ? "none" : s.deep)}, seed {s.seed}");
            sb.AppendLine($"- Totals: PASS {Count(s.cases, QAVerdict.PASS)} · FAIL {Count(s.cases, QAVerdict.FAIL)} · SKIP {Count(s.cases, QAVerdict.SKIP)} · OBSERVED {Count(s.cases, QAVerdict.OBSERVED)}");
            sb.AppendLine("- Raw verdicts only. The weekly report (`report.md`) adds KNOWN mapping and the diff with the previous run.");
            foreach (IGrouping<string, QACaseResult> group in s.cases.GroupBy(c => c.group))
            {
                sb.AppendLine();
                sb.AppendLine($"## {group.Key}");
                sb.AppendLine();
                sb.AppendLine("| ID | Pri | Case | Result | Details |");
                sb.AppendLine("|---|---|---|---|---|");
                foreach (QACaseResult c in group)
                    sb.AppendLine($"| {c.id} | {c.priority} | {Escape(c.title)} | **{c.verdict}** | {Escape(string.Join("<br>", c.notes))} |");
            }
            return sb.ToString();
        }

        private static string Escape(string text) => text.Replace("|", "\\|").Replace("\r", "").Replace("\n", " ");

        private static void Abort(Exception e)
        {
            Debug.LogError($"[QA] runner failed: {e}");
            bool cli = SessionState.GetBool(QAHarness.CliFlagKey, false);
            SessionState.SetBool(QAHarness.CliFlagKey, false);
            SessionState.SetBool(QAHarness.MenuRunKey, false);
            if (cli) EditorApplication.Exit(2);
        }

        [Serializable]
        private class QARunSummary
        {
            public string finished;
            public string unity;
            public string maxPriority;
            public string deep;
            public int seed;
            public List<string> groups = new();
            public List<QACaseResult> cases = new();
        }
    }
}
