#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QA.Automation
{
    public enum QAVerdict { PASS, FAIL, SKIP, OBSERVED }

    public enum QAPriority { P0 = 0, P1 = 1, P2 = 2 }

    [Serializable]
    public class QACaseResult
    {
        public string id;
        public string title;
        public string system;
        public string priority;
        public string group;
        public string verdict = nameof(QAVerdict.PASS);
        public float seconds;
        public List<string> notes = new();
    }

    [Serializable]
    public class QAResultFile
    {
        public string group;
        public string scene;
        public string started;
        public string finished;
        public List<QACaseResult> cases = new();
    }

    // Passed from the Editor runner to Play Mode through SessionState, which survives the domain reload.
    [Serializable]
    public class QARunConfig
    {
        public const string Key = "QA.Automation.Config";

        public string outputDir;
        public string maxPriority = nameof(QAPriority.P1);
        public bool deepAll;
        public List<string> deepSystems = new();
        public int seed = 20261031;

        public static QARunConfig Load()
        {
            string json = SessionState.GetString(Key, "");
            return string.IsNullOrEmpty(json) ? Everything() : JsonUtility.FromJson<QARunConfig>(json);
        }

        public void Save() => SessionState.SetString(Key, JsonUtility.ToJson(this));

        // A menu run with no command line: every case, results under qa-automation/reports/manual-<ts>.
        public static QARunConfig Everything() => new()
        {
            outputDir = System.IO.Path.Combine(ProjectRoot, "qa-automation", "reports", $"manual-{DateTime.Now:yyyyMMdd-HHmm}"),
            maxPriority = nameof(QAPriority.P2),
            deepAll = true,
        };

        public static string ProjectRoot => System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, ".."));

        public bool Selected(string system, QAPriority priority)
        {
            QAPriority max = Enum.TryParse(maxPriority, out QAPriority parsed) ? parsed : QAPriority.P1;
            if (priority <= max) return true;
            return priority == QAPriority.P2 && (deepAll || deepSystems.Contains(system));
        }
    }

    // One per QA scene. Runs every QASuite on its GameObject in component order, then writes
    // partial-<group>.json into the run's output folder and leaves Play Mode.
    [DefaultExecutionOrder(-10000)]
    public class QAHarness : MonoBehaviour
    {
        public const string CliFlagKey = "QA.Automation.Cli";
        public const string MenuRunKey = "QA.Automation.MenuRun";

        [SerializeField] private string group;
        [SerializeField] private float caseTimeout = 45f;

        private readonly List<QACaseResult> _results = new();
        private QACaseResult _current;
        private QARunConfig _config;
        private string _started;

        public QARunConfig Config => _config;
        public string Group => group;
        public bool InCase => _current != null;

        private void OnEnable() => Application.logMessageReceived += OnLog;

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLog;
            Time.captureFramerate = 0;
        }

        private IEnumerator Start()
        {
            _config = QARunConfig.Load();
            _started = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Application.runInBackground = true;
            // Fixed 60 fps game time: every Time.deltaTime is 1/60 whatever the machine load, so timed
            // waits and animation progress do not depend on how fast the run machine is.
            Time.captureFramerate = 60;
            // Without a cap the Editor renders as fast as it can; on 2026-11-01 that ended in a D3D11 device
            // reset (GPU timeout) that killed the run. 60 fps real time matches the fixed game time anyway.
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            UnityEngine.Random.InitState(_config.seed);

            QASuite[] suites = GetComponents<QASuite>();
            foreach (QASuite suite in suites) suite.Bind(this);
            yield return null;

            foreach (QASuite suite in suites)
            {
                IEnumerator run = suite.Run();
                yield return Drive(run, null);
                suite.Cleanup();
            }

            WriteResults();
            Time.captureFramerate = 0;
            if (SessionState.GetBool(CliFlagKey, false) || SessionState.GetBool(MenuRunKey, false))
                EditorApplication.isPlaying = false;
        }

        public IEnumerator Case(string id, string system, QAPriority priority, string title, IEnumerator body)
        {
            if (!_config.Selected(system, priority)) yield break;

            _current = new QACaseResult
            {
                id = id, title = title, system = system, priority = priority.ToString(), group = group,
            };
            _results.Add(_current);
            UnityEngine.Random.InitState(_config.seed ^ StableHash(id));
            float start = Time.realtimeSinceStartup;

            yield return Drive(body, _current);

            _current.seconds = Time.realtimeSinceStartup - start;
            Debug.Log($"[QA] {_current.verdict} {_current.id} — {_current.title}");
            _current = null;
        }

        // Steps a nested coroutine by hand so an exception or a timeout fails the case instead of
        // killing the whole run.
        private IEnumerator Drive(IEnumerator body, QACaseResult owner)
        {
            float end = Time.realtimeSinceStartup + (owner != null ? caseTimeout : float.MaxValue);
            float? pausedSince = null;
            Stack<IEnumerator> stack = new();
            stack.Push(body);
            while (stack.Count > 0)
            {
                if (owner != null && Time.realtimeSinceStartup > end)
                {
                    Fail($"timed out after {caseTimeout:0}s (real time)");
                    break;
                }
                // A UI panel left open pauses the game; every wait runs on game time and would hang.
                if (Time.timeScale == 0f)
                {
                    pausedSince ??= Time.realtimeSinceStartup;
                    if (Time.realtimeSinceStartup - pausedSince > 2f)
                    {
                        Fail("game paused (Time.timeScale = 0) for 2s — a UI panel left open; timeScale forced back to 1");
                        Time.timeScale = 1f;
                        pausedSince = null;
                    }
                }
                else pausedSince = null;

                IEnumerator top = stack.Peek();
                bool moved;
                try
                {
                    moved = top.MoveNext();
                }
                catch (Exception e)
                {
                    IEnumerable<string> frames = (e.StackTrace ?? "").Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).Take(4);
                    string message = $"exception {e.GetType().Name}: {e.Message} @ {string.Join(" <- ", frames)}";
                    if (owner != null) Fail(message);
                    else LogCase().notes.Add(message);
                    break;
                }
                if (!moved)
                {
                    stack.Pop();
                    continue;
                }
                if (top.Current is IEnumerator nested)
                {
                    stack.Push(nested);
                    continue;
                }
                yield return top.Current;
            }
        }

        public void Check(bool condition, string pass, string fail)
        {
            if (condition) Note("ok: " + pass);
            else Fail(fail);
        }

        public void Fail(string message)
        {
            if (_current == null)
            {
                LogCase().notes.Add("FAIL: " + message);
                return;
            }
            _current.verdict = nameof(QAVerdict.FAIL);
            _current.notes.Add("FAIL: " + message);
        }

        public void Skip(string message)
        {
            if (_current == null) return;
            if (_current.verdict is nameof(QAVerdict.PASS) or nameof(QAVerdict.OBSERVED)) _current.verdict = nameof(QAVerdict.SKIP);
            _current.notes.Add("SKIP: " + message);
        }

        public void Observe(string message)
        {
            if (_current == null) return;
            if (_current.verdict == nameof(QAVerdict.PASS)) _current.verdict = nameof(QAVerdict.OBSERVED);
            _current.notes.Add("OBSERVED: " + message);
        }

        public void Note(string message) => _current?.notes.Add(message);

        private void OnLog(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
            if (condition.StartsWith("[QA]")) return;
            string where = stackTrace?.Split('\n').FirstOrDefault(l => !string.IsNullOrWhiteSpace(l))?.Trim();
            string message = $"{type}: {condition.Split('\n')[0]}{(string.IsNullOrEmpty(where) ? "" : $" @ {where}")}";
            if (_current != null) Fail(message);
            else LogCase().notes.Add(message);
        }

        private QACaseResult LogCase()
        {
            QACaseResult log = _results.FirstOrDefault(r => r.id == $"QA-LOG.{group}");
            if (log != null) return log;
            log = new QACaseResult
            {
                id = $"QA-LOG.{group}", title = $"Errors logged outside a case ({group})", system = "LOG",
                priority = nameof(QAPriority.P0), group = group, verdict = nameof(QAVerdict.FAIL),
            };
            _results.Add(log);
            return log;
        }

        private void WriteResults()
        {
            QAResultFile file = new()
            {
                group = group, scene = gameObject.scene.path, started = _started,
                finished = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), cases = _results,
            };
            Directory.CreateDirectory(_config.outputDir);
            string path = System.IO.Path.Combine(_config.outputDir, $"partial-{group}.json");
            File.WriteAllText(path, JsonUtility.ToJson(file, true));
            Debug.Log($"[QA] {group}: {_results.Count} cases, {_results.Count(r => r.verdict == "FAIL")} FAIL -> {path}");
        }

        // string.GetHashCode is randomised per process on some runtimes; this one is stable.
        public static int StableHash(string text)
        {
            unchecked
            {
                int hash = 23;
                foreach (char c in text) hash = hash * 31 + c;
                return hash;
            }
        }
    }
}
#endif
