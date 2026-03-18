using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Baruah.ModelSystem.Editor
{
    public class GameModelGeneratorWindow : EditorWindow
    {
        private const string OutputPrefKey =
            "Baruah.ModelSystem.OutputFolder";

        private static string _outputFolder;

        private static List<string> _includePaths = new();
        private static List<string> _excludePaths = new();

        private static List<ModelEntry> _entries = new();

        private static Vector2 _scroll;
        
        // =========================================================
        // CONFIG
        // =========================================================

        private const string OutputFolder = "Assets/Generated Scripts";

        private static readonly Dictionary<ModelValueType, string> TemplateCache =
            new();

        // =========================================================
        // WINDOW
        // =========================================================

        [MenuItem("Tools/Game Model Generator")]
        public static void ShowWindow()
        {
            GetWindow<GameModelGeneratorWindow>("Game Model Generator");
        }

        private void OnGUI()
        {
            if (_entries == null || _entries.Count == 0)
            {
                ScanModels();
            }
            
            _outputFolder = EditorPrefs.GetString(OutputPrefKey, OutputFolder);
            DrawTopPanel();

            GUILayout.Space(10);

            DrawOutputFolder();

            GUILayout.Space(5);

            DrawIncludePaths();
            DrawExcludePaths();

            GUILayout.Space(10);

            DrawResultsTable();
        }
        
        private static void DrawResultsTable()
        {
            GUILayout.Label("Scripts To Generate", EditorStyles.boldLabel);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            foreach (var modelEntry in _entries)
            {
                var entry = modelEntry;
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(entry.Type.Name, GUILayout.Width(180));
                GUILayout.Label(entry.Type.Namespace ?? "", GUILayout.ExpandWidth(true));

                if (GUILayout.Button("Generate", GUILayout.Width(90)))
                {
                    GenerateEntry(entry);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
        
        private static void GenerateEntry(ModelEntry entry)
        {
            LoadTemplates();
            
            if (!TemplateCache.TryGetValue(entry.Attribute.ValueType, out var template))
                return;

            string modelPath = GenerateModel(entry.Type, entry.Attribute.ValueType, template);
            string queryPath = GenerateQueryClass(entry.Type);
            
            AssetDatabase.ImportAsset(modelPath, ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.ImportAsset(queryPath, ImportAssetOptions.ForceSynchronousImport);
        }
        
        private static void DrawTopPanel()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate All", GUILayout.Height(30)))
            {
                GenerateAll();
            }

            if (GUILayout.Button("Rescan", GUILayout.Height(30), GUILayout.Width(80)))
            {
                ScanModels();
            }

            EditorGUILayout.EndHorizontal();
        }
        
        [Serializable]
        private class StringListWrapper
        {
            public List<string> Items = new();
        }
        
        private static void DrawIncludePaths()
            => _includePaths = DrawPathList("Include Paths", "includePaths");

        private static void DrawExcludePaths()
            => _excludePaths = DrawPathList("Exclude Paths", "excludePaths");
        
        private static List<string> DrawPathList(string label, string key)
        {
            string foldKey = $"{key}-foldout";

            bool foldout = GetFoldBool(foldKey);

            List<string> list = LoadList(key);

            bool newFoldout = EditorGUILayout.Foldout(foldout, label, true);

            if (newFoldout != foldout)
                EditorPrefs.SetBool(foldKey, newFoldout);

            if (!newFoldout)
                return list;

            bool changed = false;

            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                string newValue = EditorGUILayout.TextField(list[i]);

                if (newValue != list[i])
                {
                    list[i] = newValue;
                    changed = true;
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    list.RemoveAt(i);
                    changed = true;
                    i--;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button($"Add {label}"))
            {
                list.Add("Assets/");
                changed = true;
            }

            if (changed)
            {
                EditorPrefs.SetString(
                    key,
                    JsonUtility.ToJson(new StringListWrapper { Items = list }));
            }

            return list;
        }

        private static bool GetFoldBool(string key)
        {
            return EditorPrefs.GetBool(key, false);
        }
        
        private static List<string> LoadList(string key)
        {
            if (!EditorPrefs.HasKey(key))
                return new List<string>();

            var json = EditorPrefs.GetString(key);

            if (string.IsNullOrEmpty(json))
                return new List<string>();

            try
            {
                return JsonUtility
                    .FromJson<StringListWrapper>(json)
                    ?.Items ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
        
        private static void ScanModels()
        {
            _entries.Clear();

            var types = TypeCache.GetTypesWithAttribute<GameModelAttribute>();

            foreach (var type in types)
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                var attr = type.GetCustomAttribute<GameModelAttribute>();

                _entries.Add(new ModelEntry
                {
                    Type = type,
                    Attribute = attr
                });
            }

            Debug.Log($"Found {_entries.Count} models.");
        }
        
        private static bool IsPathAllowed(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (_includePaths.Count > 0 &&
                !_includePaths.Any(p => path.StartsWith(p)))
                return false;

            if (_excludePaths.Any(p => path.StartsWith(p)))
                return false;

            return true;
        }
        
        private static void GenerateFromPath(string includePath)
        {
            if (string.IsNullOrEmpty(includePath))
                return;

            EnsureOutputFolder(_outputFolder);
            LoadTemplates();

            var types = TypeCache.GetTypesWithAttribute<GameModelAttribute>();

            int count = 0;

            foreach (var type in types)
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                string scriptPath = GetScriptPath(type);

                if (string.IsNullOrEmpty(scriptPath))
                    continue;

                if (!scriptPath.StartsWith(includePath))
                    continue;

                if (_excludePaths.Any(p => scriptPath.StartsWith(p)))
                    continue;

                var attr = type.GetCustomAttribute<GameModelAttribute>();

                if (!TemplateCache.TryGetValue(attr.ValueType, out var template))
                    continue;

                GenerateModel(type, attr.ValueType, template);
                GenerateQueryClass(type);

                count++;
            }

            AssetDatabase.Refresh();

            Debug.Log($"Generated {count} model(s) from {includePath}");
        }
        
        private static string GetScriptPath(Type type)
        {
            var script = AssetDatabase
                .FindAssets($"t:MonoScript {type.Name}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(p => AssetDatabase.LoadAssetAtPath<MonoScript>(p))
                .FirstOrDefault(s => s != null && s.GetClass() == type);

            return script != null
                ? AssetDatabase.GetAssetPath(script)
                : null;
        }
        
        private static void DrawOutputFolder()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Output Folder", GUILayout.Width(100));
            _outputFolder = EditorGUILayout.TextField(_outputFolder);

            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string path = EditorUtility.OpenFolderPanel(
                    "Select Output Folder",
                    Application.dataPath,
                    "");

                if (!string.IsNullOrEmpty(path) &&
                    path.StartsWith(Application.dataPath))
                {
                    _outputFolder = "Assets" +
                                    path.Substring(Application.dataPath.Length);

                    EditorPrefs.SetString(OutputPrefKey, _outputFolder);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        // =========================================================
        // MAIN PIPELINE
        // =========================================================

        private static void GenerateAll()
        {
            LoadTemplates();

            var types = TypeCache.GetTypesWithAttribute<GameModelAttribute>();

            int count = 0;

            foreach (var type in types)
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                var attr = type.GetCustomAttribute<GameModelAttribute>();

                if (!TemplateCache.TryGetValue(attr.ValueType, out var template))
                {
                    Debug.LogError($"No template loaded for {attr.ValueType}");
                    continue;
                }

                GenerateModel(type, attr.ValueType, template);
                GenerateQueryClass(type);
                count++;
            }

            AssetDatabase.Refresh();

            Debug.Log($"GameModelGenerator: Generated {count} model(s).");
        }

        // =========================================================
        // TEMPLATE LOADING (USER → FALLBACK)
        // =========================================================

        private static void LoadTemplates()
        {
            TemplateCache.Clear();

            LoadTemplatesFromFolder();

            if (TemplateCache.Count == 0)
            {
                Debug.LogError("No templates found.");
            }
        }

        private static void LoadTemplatesFromFolder()
        {
            var paths = AssetDatabase.FindAssets(
                    "t:TextAsset")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => (path, priority: path.Contains("com.arijeet.modelSystem") ? 1 : 0))
                .OrderBy(x => x.priority)
                .Select(x => x.path);

            foreach (string path in paths)
            {
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

                if (asset == null)
                    continue;

                if (asset.name.Contains("SingleModelTemplate") &&
                    !TemplateCache.ContainsKey(ModelValueType.Single))
                {
                    TemplateCache[ModelValueType.Single] = asset.text;
                }
                else if (asset.name.Contains("ListModelTemplate") &&
                         !TemplateCache.ContainsKey(ModelValueType.List))
                {
                    TemplateCache[ModelValueType.List] = asset.text;
                }
            }
        }

        // =========================================================
        // MODEL GENERATION
        // =========================================================

        private static string GenerateModel(Type dataType, ModelValueType kind, string template)
        {
            string modelName = dataType.Name + "Model";
            string filePath = System.IO.Path.Combine(_outputFolder, modelName + ".cs");

            string accessors =
                kind == ModelValueType.Single
                    ? GenerateSingleAccessors(dataType)
                    : GenerateListAccessors(dataType);

            string namespaceName = dataType.Namespace;

            string result = template
                .Replace("{{MODEL_NAME}}", modelName)
                .Replace("{{DATA_TYPE}}", dataType.FullName)
                .Replace("{{ACCESSORS}}", accessors)
                .Replace("{{NAMESPACE}}", namespaceName ?? "")
                .Replace("{{QUERY_NAME}}", dataType.Name + "Query");

            if (string.IsNullOrEmpty(namespaceName))
            {
                result = RemoveNamespaceBlock(result);
            }

            System.IO.File.WriteAllText(filePath, result);
            return filePath;
        }

        private static string GenerateQueryClass(Type dataType)
        {
            string modelName = dataType.Name + "Model";
            string queryName = dataType.Name + "Query";
            string filePath = System.IO.Path.Combine(
                _outputFolder,
                queryName + ".cs");

            var sb = new StringBuilder();

            string ns = dataType.Namespace;

            if (!string.IsNullOrEmpty(ns))
            {
                sb.AppendLine($"namespace {ns}");
                sb.AppendLine("{");
            }

            sb.AppendLine(
                $@"    public sealed class {queryName} 
        : ModelQuery<{dataType.FullName}>
    {{
        public {queryName}(System.Collections.Generic.IReadOnlyList<{dataType.FullName}> source)
            : base(source) {{ }}
");

            var fields = dataType.GetFields(
                BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                GenerateQueryMethods(sb, dataType, field);
            }
            
            
            
            sb.AppendLine($@"        public {queryName} Where(System.Func<{dataType.Name}, bool> predicate)
        {{
            AddCondition(predicate);
            return this;
        }}
");

            sb.AppendLine("    }");

            if (!string.IsNullOrEmpty(ns))
                sb.AppendLine("}");

            System.IO.File.WriteAllText(filePath, sb.ToString());
            
            return filePath;
        }

        private static void GenerateQueryMethods(
            StringBuilder sb,
            Type dataType,
            FieldInfo field)
        {
            string typeName = GetTypeName(field.FieldType);
            string name = field.Name;
            string pascal = char.ToUpper(name[0]) + name.Substring(1);
            string queryName = dataType.Name + "Query";

            // Equality (all types)
            sb.AppendLine(
                $@"        public {queryName} {pascal}Equals({typeName} value)
        {{
            AddCondition(i => i.{name} == value);
            return this;
        }}
");

            // Numeric comparisons
            if (IsNumeric(field.FieldType))
            {
                sb.AppendLine(
                    $@"        public {queryName} {pascal}GreaterThan({typeName} value)
        {{
            AddCondition(i => i.{name} > value);
            return this;
        }}

        public {queryName} {pascal}LessThan({typeName} value)
        {{
            AddCondition(i => i.{name} < value);
            return this;
        }}

        public {queryName} {pascal}GreaterThanEqualTo({typeName} value)
        {{
            AddCondition(i => i.{name} >= value);
            return this;
        }}

        public {queryName} {pascal}LessThanEqualTo({typeName} value)
        {{
            AddCondition(i => i.{name} <= value);
            return this;
        }}
");
            }

            // String helpers
            if (field.FieldType == typeof(string))
            {
                sb.AppendLine(
                    $@"        public {queryName} {pascal}Contains(string value)
        {{
            AddCondition(i => i.{name} != null && i.{name}.Contains(value));
            return this;
        }}
");
            }
        }

        private static bool IsNumeric(Type type)
        {
            return type == typeof(int) ||
                   type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(long) ||
                   type == typeof(short) ||
                   type == typeof(byte);
        }

        // =========================================================
        // ACCESSOR GENERATION — SINGLE
        // =========================================================

        private static string GenerateSingleAccessors(Type dataType)
        {
            var sb = new StringBuilder();

            var fields = dataType.GetFields(
                BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                string typeName = GetTypeName(field.FieldType);
                string name = field.Name;
                string pascal = UpperFirst(name);

                sb.AppendLine($"        public {typeName} Get{pascal}() => Value.{name};");
                sb.AppendLine();

                sb.AppendLine($"        public void Set{pascal}({typeName} value)");
                sb.AppendLine("        {");
                sb.AppendLine($"            if (Equals(Value.{name}, value)) return;");
                sb.AppendLine($"            Value.{name} = value;");
                sb.AppendLine($"            On{pascal}Changed?.Invoke(value);");
                sb.AppendLine("        }");
                sb.AppendLine();

                sb.AppendLine($"        public event Action<{typeName}> On{pascal}Changed;");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        // =========================================================
        // ACCESSOR GENERATION — LIST
        // =========================================================

        private static string GenerateListAccessors(Type dataType)
        {
            var sb = new StringBuilder();

            var fields = dataType.GetFields(
                BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                string typeName = GetTypeName(field.FieldType);
                string name = field.Name;
                string pascal = UpperFirst(name);

                sb.AppendLine($"        public IEnumerable<{dataType.FullName}> FindBy{pascal}({typeName} value)");
                sb.AppendLine("        {");
                sb.AppendLine("            foreach (var item in _items)");
                sb.AppendLine($"                if (Equals(item.{name}, value))");
                sb.AppendLine("                    yield return item;");
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        // =========================================================
        // HELPERS
        // =========================================================

        private static void EnsureOutputFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            string parent = Path.GetDirectoryName(path);
            string name = Path.GetFileName(path);

            if (!AssetDatabase.IsValidFolder(parent))
                EnsureOutputFolder(parent);

            AssetDatabase.CreateFolder(parent, name);
        }

        private static string RemoveNamespaceBlock(string text)
        {
            const string keyword = "namespace ";

            int start = text.IndexOf(keyword, StringComparison.Ordinal);
            if (start < 0)
                return text;

            int openBrace = text.IndexOf('{', start);
            int closeBrace = text.LastIndexOf('}');

            if (openBrace < 0 || closeBrace < 0)
                return text;

            return text.Substring(openBrace + 1, closeBrace - openBrace - 1).Trim();
        }

        private static string UpperFirst(string s)
            => char.ToUpper(s[0]) + s.Substring(1);

        private static string GetTypeName(Type type)
        {
            if (!type.IsGenericType)
                return type.FullName;

            var genericType = type.GetGenericTypeDefinition();
            var args = type.GetGenericArguments().Select(GetTypeName);

            return $"{genericType.FullName.Split('`')[0]}<{string.Join(", ", args)}>";
        }
    }
    
    class ModelEntry
    {
        public Type Type;
        public GameModelAttribute Attribute;
    }
}
