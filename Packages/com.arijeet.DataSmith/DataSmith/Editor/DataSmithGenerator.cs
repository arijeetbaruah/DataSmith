using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Baruah.DataSmith.Editor
{
    public static class DataSmithGenerator
    {
        public static void GenerateEntry(ModelEntry entry, string outputFolder)
        {
            var templates = LoadTemplates();

            if (!templates.TryGetValue(entry.Attribute.ValueType, out var template))
                return;

            string modelCode  = BuildModel(entry, template);

            string queryCode  = BuildQuery(entry);
            
            string modelPath = Path.Combine(outputFolder,
                entry.Type.Name + "Model.cs");

            string queryPath = Path.Combine(outputFolder,
                entry.Type.Name + "Query.cs");
            
            File.WriteAllText(modelPath, modelCode);
            File.WriteAllText(queryPath, queryCode);

            AssetDatabase.ImportAsset(modelPath, ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.ImportAsset(queryPath, ImportAssetOptions.ForceSynchronousImport);
        }
        
        public static void GenerateAll(IEnumerable<ModelEntry> entries, string outputFolder)
        {
            foreach (var entry in entries)
                GenerateEntry(entry, outputFolder);

            AssetDatabase.Refresh();
        }

        private static IEnumerator GenerateAllAsync(IEnumerable<ModelEntry> entries, string outputFolder)
        {
            EditorUtility.DisplayProgressBar("Generating Models", "Generating Models", 0);
            
            int count = 0;
            foreach (var entry in entries)
            {
                float progress = (float) count / (float) entries.Count();
                if (EditorUtility.DisplayCancelableProgressBar("Generating Models", $"Generating {entry.Type.FullName}", progress))
                {
                    break;
                }
                
                GenerateEntry(entry, outputFolder);
                
                yield return null;
                count++;
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        } 
        
        public static Dictionary<ModelValueType, string> LoadTemplates()
        {
            var cache = new Dictionary<ModelValueType, string>();

            var paths = AssetDatabase.FindAssets("t:TextAsset")
                .Select(g => AssetDatabase.GUIDToAssetPath(g));

            foreach (var path in paths)
            {
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                if (asset == null) continue;

                if (asset.name.Contains("SingleModelTemplate"))
                    cache[ModelValueType.Single] = asset.text;

                if (asset.name.Contains("ListModelTemplate"))
                    cache[ModelValueType.List] = asset.text;
            }

            return cache;
        }
        
        public static string BuildModel(ModelEntry entry, string template)
        {
            var type = entry.Type;
            var kind = entry.Attribute.ValueType;

            string accessors =
                kind == ModelValueType.Single
                    ? BuildSingleAccessors(type)
                    : BuildListAccessors(type);

            string namespaceName = type.Namespace;

            string result = template
                .Replace("{{MODEL_NAME}}", type.Name + "Model")
                .Replace("{{DATA_TYPE}}", type.FullName)
                .Replace("{{ACCESSORS}}", accessors)
                .Replace("{{NAMESPACE}}", namespaceName ?? "")
                .Replace("{{QUERY_NAME}}", type.Name + "Query");

            if (string.IsNullOrEmpty(namespaceName))
                result = RemoveNamespaceBlock(result);

            return result;
        }
        
        public static string BuildQuery(ModelEntry entry)
        {
            var type = entry.Type;
            string queryName = type.Name + "Query";

            var sb = new System.Text.StringBuilder();

            sb.AppendLine("/* Auto-generated. DO NOT MODIFY */");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(type.Namespace))
            {
                sb.AppendLine($"namespace {type.Namespace}");
                sb.AppendLine("{");
            }

            sb.AppendLine(
                $@"    public sealed class {queryName} 
        : ModelQuery<{type.FullName}>
    {{
        public {queryName}(System.Collections.Generic.IReadOnlyList<{type.FullName}> source)
            : base(source) {{ }}
");

            foreach (var field in type.GetFields(
                         BindingFlags.Public | BindingFlags.Instance))
            {
                BuildQueryMethods(sb, type, field);
            }

            sb.AppendLine(
                $@"        public {queryName} Where(System.Func<{type.Name}, bool> predicate)
        {{
            AddCondition(predicate);
            return this;
        }}
");

            sb.AppendLine("    }");

            if (!string.IsNullOrEmpty(type.Namespace))
                sb.AppendLine("}");

            return sb.ToString();
        }
        
        private static string BuildSingleAccessors(Type dataType)
        {
            var sb = new System.Text.StringBuilder();

            foreach (var field in dataType.GetFields(
                         BindingFlags.Public | BindingFlags.Instance))
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
        
        private static string BuildListAccessors(Type dataType)
        {
            var sb = new System.Text.StringBuilder();

            foreach (var field in dataType.GetFields(
                         BindingFlags.Public | BindingFlags.Instance))
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
        
        private static void BuildQueryMethods(System.Text.StringBuilder sb, Type dataType, FieldInfo field)
        {
            string typeName = GetTypeName(field.FieldType);
            string name = field.Name;
            string pascal = UpperFirst(name);
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
        
        private static string UpperFirst(string s)
            => char.ToUpper(s[0]) + s.Substring(1);

        private static string RemoveNamespaceBlock(string text)
        {
            const string keyword = "namespace ";

            int start = text.IndexOf(keyword, StringComparison.Ordinal);
            if (start < 0) return text;

            int open = text.IndexOf('{', start);
            int close = text.LastIndexOf('}');

            return text.Substring(open + 1, close - open - 1).Trim();
        }

        private static string GetTypeName(Type type)
        {
            if (!type.IsGenericType)
                return type.FullName;

            var generic = type.GetGenericTypeDefinition();
            var args = type.GetGenericArguments()
                .Select(GetTypeName);

            return $"{generic.FullName.Split('`')[0]}<{string.Join(", ", args)}>";
        }
    }
}
