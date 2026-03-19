using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Baruah.DataSmith.Editor
{
    public static class DataSmithScanner
    {
        public static List<ModelEntry> Scan(IReadOnlyList<string> includePaths, IReadOnlyList<string> excludePaths)
        {
            var result = new List<ModelEntry>();

            var types = TypeCache.GetTypesWithAttribute<GameModelAttribute>();

            foreach (var type in types)
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                string path = GetScriptPath(type);

                if (string.IsNullOrEmpty(path))
                    continue;

                if (!IsAllowed(path, includePaths, excludePaths))
                    continue;

                var attr = type.GetCustomAttribute<GameModelAttribute>();

                result.Add(new ModelEntry
                {
                    Type = type,
                    Path = path,
                    Attribute = attr
                });
            }

            return result;
        }
        
        public static string GetScriptPath(System.Type type)
        {
            var guids = AssetDatabase.FindAssets("t:MonoScript");

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                if (script == null) continue;

                if (script.GetClass() == type)
                    return path;

                if (script.text.Contains($"class {type.Name}"))
                    return path;
            }

            return null;
        }
        
        public static bool IsAllowed(string path, IReadOnlyList<string> include, IReadOnlyList<string> exclude)
        {
            path = path.Replace('\\', '/');

            if (include != null && include.Count > 0)
            {
                if (!include.Any(p => GlobMatch(path, p)))
                    return false;
            }

            if (exclude != null && exclude.Count > 0)
            {
                if (exclude.Any(p => GlobMatch(path, p)))
                    return false;
            }

            return true;
        }

        private static bool GlobMatch(string path, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return false;

            pattern = pattern.Replace('\\', '/');

            string regex = "^" + RegexEscape(pattern) + "$";

            return System.Text.RegularExpressions.Regex
                .IsMatch(path, regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        private static string RegexEscape(string pattern)
        {
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < pattern.Length; i++)
            {
                char c = pattern[i];

                if (c == '*')
                {
                    bool doubleStar = i + 1 < pattern.Length && pattern[i + 1] == '*';
                    sb.Append(doubleStar ? ".*" : "[^/]*");
                    if (doubleStar) i++;
                }
                else
                {
                    sb.Append(System.Text.RegularExpressions.Regex.Escape(c.ToString()));
                }
            }

            return sb.ToString();
        }
    }
}
