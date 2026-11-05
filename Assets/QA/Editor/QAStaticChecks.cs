using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace QA.Automation.Editor
{
    // Edit Mode checks on assets and source text — no Play Mode, no scene. Group STATIC.
    // Each finding is a class of defect this project has shipped before (see the case catalog).
    public static class QAStaticChecks
    {
        private const string Group = "STATIC";

        // Unity's own keys in a MonoBehaviour / ScriptableObject YAML block; never orphans.
        private static readonly HashSet<string> UnityKeys = new()
        {
            "m_ObjectHideFlags", "m_CorrespondingSourceObject", "m_PrefabInstance", "m_PrefabAsset", "m_GameObject",
            "m_Enabled", "m_EditorHideFlags", "m_Script", "m_Name", "m_EditorClassIdentifier", "serializedVersion",
            "references", "version", "RefIds", "rid", "type", "data", "class", "ns", "asm",
        };

        private static readonly Regex YamlKey = new(@"^(\s*)(?:-\s+)?([A-Za-z_<>][A-Za-z0-9_<>]*):", RegexOptions.Compiled);
        private static readonly Regex ScriptRef = new(@"m_Script:\s*\{fileID:\s*11500000,\s*guid:\s*([0-9a-f]{32})", RegexOptions.Compiled);
        private static readonly Regex AnyGuid = new(@"guid:\s*([0-9a-f]{32})", RegexOptions.Compiled);

        public static QAResultFile Run(QARunConfig config)
        {
            QAResultFile file = new() { group = Group, scene = "(Edit Mode)", started = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
            Add(file, config, "QA-STATIC-01", QAPriority.P0, "No asset or prefab references a missing script", MissingScripts);
            Add(file, config, "QA-STATIC-02", QAPriority.P1, "No ScriptableObject under Assets/SO references a deleted asset", MissingAssetRefs);
            Add(file, config, "QA-STATIC-03", QAPriority.P1, "No serialized key left behind by a field rename (missing [FormerlySerializedAs])", OrphanKeys);
            Add(file, config, "QA-STATIC-04", QAPriority.P0, "No runtime stat modifier saved into a stat asset", SavedModifiers);
            Add(file, config, "QA-STATIC-05", QAPriority.P0, "Runtime scripts do not use UnityEditor outside #if UNITY_EDITOR", EditorUsings);
            Add(file, config, "QA-STATIC-06", QAPriority.P0, "Every enabled build scene exists", BuildScenes);
            Add(file, config, "QA-STATIC-07", QAPriority.P1, "Every enemy role has a weapon, a move speed and a consistent attack range", EnemyRoleData);
            file.finished = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return file;
        }

        private static void Add(QAResultFile file, QARunConfig config, string id, QAPriority priority, string title, Action<QACaseResult> body)
        {
            if (!config.Selected("STATIC", priority)) return;
            QACaseResult result = new() { id = id, title = title, system = "STATIC", priority = priority.ToString(), group = Group };
            float start = Time.realtimeSinceStartup;
            try
            {
                body(result);
            }
            catch (Exception e)
            {
                Fail(result, $"exception {e.GetType().Name}: {e.Message}");
            }
            result.seconds = Time.realtimeSinceStartup - start;
            file.cases.Add(result);
            Debug.Log($"[QA] {result.verdict} {result.id} — {result.title}");
        }

        private static void Fail(QACaseResult r, string message)
        {
            r.verdict = nameof(QAVerdict.FAIL);
            r.notes.Add("FAIL: " + message);
        }

        private static string Root => QARunConfig.ProjectRoot;

        private static IEnumerable<string> Files(string folder, params string[] patterns)
        {
            string dir = System.IO.Path.Combine(Root, folder);
            if (!Directory.Exists(dir)) return Enumerable.Empty<string>();
            return patterns.SelectMany(p => Directory.GetFiles(dir, p, SearchOption.AllDirectories)).OrderBy(f => f, StringComparer.Ordinal);
        }

        private static string Rel(string full) => full.Substring(Root.Length + 1).Replace('\\', '/');

        private static bool GuidExists(string guid) => !string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guid));

        private static void Report(QACaseResult r, List<string> findings, string okMessage, int limit = 2000)
        {
            if (findings.Count == 0)
            {
                r.notes.Add("ok: " + okMessage);
                return;
            }
            Fail(r, $"{findings.Count} finding(s)");
            r.notes.AddRange(findings.Take(limit));
            if (findings.Count > limit) r.notes.Add($"... {findings.Count - limit} more");
        }

        private static void MissingScripts(QACaseResult r)
        {
            List<string> findings = new();
            int scanned = 0;
            foreach (string file in Files("Assets", "*.asset", "*.prefab", "*.unity"))
            {
                if (Rel(file).StartsWith("Assets/QA/")) continue;
                scanned++;
                foreach (Match m in ScriptRef.Matches(File.ReadAllText(file)))
                    if (!GuidExists(m.Groups[1].Value)) findings.Add($"{Rel(file)}: script guid {m.Groups[1].Value}");
            }
            Report(r, findings.Distinct().ToList(), $"{scanned} files, every m_Script resolves");
        }

        private static void MissingAssetRefs(QACaseResult r)
        {
            List<string> findings = new();
            int scanned = 0;
            foreach (string file in Files("Assets/SO", "*.asset"))
            {
                scanned++;
                string text = File.ReadAllText(file);
                foreach (Match m in AnyGuid.Matches(text))
                {
                    string guid = m.Groups[1].Value;
                    if (guid.StartsWith("0000000000000000")) continue;
                    if (!GuidExists(guid)) findings.Add($"{Rel(file)}: guid {guid}");
                }
            }
            Report(r, findings.Distinct().ToList(), $"{scanned} SO assets, every guid resolves");
        }

        // A YAML key that no serialized property of the asset's type carries is data Unity no longer reads:
        // the field was renamed or removed without [FormerlySerializedAs].
        private static void OrphanKeys(QACaseResult r)
        {
            List<string> findings = new();
            int scanned = 0;
            foreach (string file in Files("Assets/SO", "*.asset"))
            {
                string rel = Rel(file);
                ScriptableObject asset = AssetDatabase.LoadMainAssetAtPath(rel) as ScriptableObject;
                if (asset == null) continue;
                scanned++;

                HashSet<string> known = new(StringComparer.Ordinal);
                SerializedObject so = new(asset);
                SerializedProperty it = so.GetIterator();
                while (it.Next(true)) known.Add(it.name);
                known.UnionWith(FormerNames(asset.GetType()));

                HashSet<string> orphans = new(StringComparer.Ordinal);
                bool inBlock = false;
                foreach (string line in File.ReadLines(file))
                {
                    if (line.StartsWith("--- ")) { inBlock = false; continue; }
                    if (line.StartsWith("MonoBehaviour:")) { inBlock = true; continue; }
                    if (!inBlock) continue;
                    Match m = YamlKey.Match(line);
                    if (!m.Success) continue;
                    string key = m.Groups[2].Value;
                    if (UnityKeys.Contains(key) || key.StartsWith("m_") || known.Contains(key)) continue;
                    orphans.Add(key);
                }
                // One line per key, so the static baseline and the known-issue patterns can match each one.
                foreach (string key in orphans.OrderBy(k => k, StringComparer.Ordinal)) findings.Add($"{rel} ({asset.GetType().Name}): {key}");
                if (scanned % 50 == 0) EditorUtility.UnloadUnusedAssetsImmediate();
            }
            Report(r, findings, $"{scanned} SO assets, no orphaned keys");
        }

        // Old names declared with [FormerlySerializedAs] are still read by Unity, so they are not orphans.
        private static readonly Dictionary<Type, HashSet<string>> FormerNamesCache = new();

        private static HashSet<string> FormerNames(Type type, int depth = 0)
        {
            if (FormerNamesCache.TryGetValue(type, out HashSet<string> cached)) return cached;
            HashSet<string> names = new(StringComparer.Ordinal);
            FormerNamesCache[type] = names;
            const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public |
                                                         System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.DeclaredOnly;
            for (Type t = type; t != null && t != typeof(ScriptableObject) && t != typeof(object); t = t.BaseType)
            {
                foreach (System.Reflection.FieldInfo field in t.GetFields(flags))
                {
                    foreach (UnityEngine.Serialization.FormerlySerializedAsAttribute a in field.GetCustomAttributes(typeof(UnityEngine.Serialization.FormerlySerializedAsAttribute), true))
                        names.Add(a.oldName);
                    Type fieldType = field.FieldType.IsArray ? field.FieldType.GetElementType()
                        : field.FieldType.IsGenericType ? field.FieldType.GetGenericArguments()[0] : field.FieldType;
                    if (depth < 3 && fieldType != null && fieldType.IsSerializable && !fieldType.IsPrimitive && fieldType != typeof(string)
                        && !typeof(UnityEngine.Object).IsAssignableFrom(fieldType) && fieldType.Assembly == type.Assembly)
                        names.UnionWith(FormerNames(fieldType, depth + 1));
                }
            }
            return names;
        }

        // BUG-063 / BUG-115: Stat.modifiers is serialized in the Editor, so a runtime buff can be saved into
        // a committed stat asset. A non-empty `modifiers:` list under Assets/SO/Stat is that leak.
        private static void SavedModifiers(QACaseResult r)
        {
            List<string> findings = new();
            foreach (string file in Files("Assets/SO", "*.asset"))
            {
                string[] lines = File.ReadAllLines(file);
                if (!lines.Any(l => l.Contains("baseValue:"))) continue;
                int count = 0;
                for (int i = 0; i < lines.Length; i++)
                {
                    Match m = Regex.Match(lines[i], @"^(\s*)modifiers:\s*$");
                    if (!m.Success) continue;
                    string indent = m.Groups[1].Value;
                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        if (lines[j].StartsWith(indent + "- ")) count++;
                        else if (!lines[j].StartsWith(indent + "  ")) break;
                    }
                }
                if (count > 0) findings.Add($"{Rel(file)}: {count} saved modifier(s)");
            }
            Report(r, findings, "no stat asset carries saved modifiers");
        }

        private static void EditorUsings(QACaseResult r)
        {
            List<string> findings = new();
            foreach (string file in Files("Assets", "*.cs"))
            {
                string rel = Rel(file);
                if (rel.Contains("/Editor/") || rel.StartsWith("Assets/QA/") || rel.StartsWith("Assets/Plugins/")) continue;
                int depth = 0;
                bool editorBlock = false;
                Stack<bool> guards = new();
                int lineNo = 0;
                foreach (string raw in File.ReadLines(file))
                {
                    lineNo++;
                    string line = raw.Trim();
                    if (line.StartsWith("#if"))
                    {
                        guards.Push(line.Contains("UNITY_EDITOR"));
                        depth++;
                    }
                    else if (line.StartsWith("#endif") && guards.Count > 0)
                    {
                        guards.Pop();
                        depth--;
                    }
                    editorBlock = guards.Any(g => g);
                    if (!editorBlock && Regex.IsMatch(line, @"^using\s+UnityEditor(\.|;)"))
                        findings.Add($"{rel}:{lineNo}: {line}");
                }
            }
            Report(r, findings, "no unguarded `using UnityEditor` in runtime scripts");
        }

        private static void BuildScenes(QACaseResult r)
        {
            List<string> findings = new();
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            foreach (EditorBuildSettingsScene scene in scenes.Where(s => s.enabled))
                if (!File.Exists(System.IO.Path.Combine(Root, scene.path))) findings.Add($"missing: {scene.path}");
            r.notes.Add("build scenes: " + string.Join(", ", scenes.Where(s => s.enabled).Select(s => s.path)));
            if (!scenes.Any(s => s.enabled)) findings.Add("no enabled build scene");
            Report(r, findings, $"{scenes.Count(s => s.enabled)} enabled scenes exist");
        }

        // Since 6094aa10 enemy move speed and attack range come from the role's stat asset
        // (StatType.MoveSpeed / MaxRangeAttack / MinRangeAttack) and detection from EntityData.rangeCheckFieldOfView.
        // A zero there means an enemy that never moves, never attacks or never sees the player.
        private static void EnemyRoleData(QACaseResult r)
        {
            List<string> findings = new();
            List<string> info = new();
            string[] guids = AssetDatabase.FindAssets("t:EntityData", new[] { "Assets/SO/Enemy/Role" });
            if (guids.Length == 0)
            {
                r.verdict = nameof(QAVerdict.SKIP);
                r.notes.Add("SKIP: no EntityData under Assets/SO/Enemy/Role");
                return;
            }
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                EntityData data = AssetDatabase.LoadAssetAtPath<EntityData>(path);
                if (data == null) continue;
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                if (data.Weapon == null) findings.Add($"{name}: no Weapon");
                if (data.Stats == null)
                {
                    findings.Add($"{name}: no stat asset");
                    continue;
                }
                BaseStatsSO copy = UnityEngine.Object.Instantiate(data.Stats);
                try
                {
                    float move = copy.GetStatValue(StatType.MoveSpeed);
                    float max = copy.GetStatValue(StatType.MaxRangeAttack);
                    float min = copy.GetStatValue(StatType.MinRangeAttack);
                    float fov = data.RangeCheckFieldOfView;
                    info.Add($"{name}: MoveSpeed {move:0.##}, range {min:0.##}-{max:0.##}, FOV {fov:0.##}");
                    if (move <= 0f) findings.Add($"{name}: MoveSpeed {move:0.##} (never moves)");
                    if (max <= 0f) findings.Add($"{name}: MaxRangeAttack {max:0.##} (never attacks)");
                    if (min < 0f || min >= max) findings.Add($"{name}: MinRangeAttack {min:0.##} not below MaxRangeAttack {max:0.##}");
                    if (fov < max) findings.Add($"{name}: rangeCheckFieldOfView {fov:0.##} below MaxRangeAttack {max:0.##}");
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(copy);
                }
            }
            Report(r, findings, $"{guids.Length} role EntityData assets consistent");
            if (findings.Count == 0) r.notes.AddRange(info);
        }
    }
}
