using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Baruah.DataSmith.Editor
{
    public static class DataSmithScanner
    {
        /// <summary>
        /// Scans assemblies for non-abstract classes annotated with <c>GameModelAttribute</c> and returns metadata entries linking each type to its script asset.
        /// </summary>
        /// <param name="includePaths">Optional glob-style path patterns; when provided, only script paths that match at least one pattern are included.</param>
        /// <param name="excludePaths">Optional glob-style path patterns; script paths that match any pattern are excluded.</param>
        /// <returns>A list of <see cref="ModelEntry"/> values containing the discovered type, its script asset path, and the associated <c>GameModelAttribute</c>.</returns>
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
        
        /// <summary>
        /// Locate the Unity MonoScript asset path that defines the specified runtime type.
        /// </summary>
        /// <param name="type">The System.Type to locate among the project's MonoScript assets.</param>
        /// <returns>The asset path of the MonoScript that declares or contains the specified type, or null if no matching script is found.</returns>
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
        
        /// <summary>
        /// Determines whether a normalized asset path is permitted by optional include and exclude glob patterns.
        /// </summary>
        /// <param name="path">The asset path to test; backslashes are treated as forward slashes before matching.</param>
        /// <param name="include">Optional list of glob-style include patterns; if provided, the path must match at least one pattern.</param>
        /// <param name="exclude">Optional list of glob-style exclude patterns; if provided, the path must not match any pattern.</param>
        /// <returns>`true` if the path passes the include and exclude constraints, `false` otherwise.</returns>
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

        /// <summary>
        /// Determines whether a file path matches a glob-style pattern.
        /// </summary>
        /// <param name="path">The file path to test (may contain forward or back slashes).</param>
        /// <param name="pattern">A glob pattern where `*` matches any characters except `/` and `**` matches across directory separators; backslashes are treated as `/`.</param>
        /// <returns>`true` if the path matches the pattern, `false` otherwise.</returns>
        private static bool GlobMatch(string path, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return false;

            pattern = pattern.Replace('\\', '/');

            string regex = "^" + RegexEscape(pattern) + "$";

            return System.Text.RegularExpressions.Regex
                .IsMatch(path, regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Converts a glob-style pattern into a regular expression fragment, treating
        /// `*` as "match any characters except '/'" and `**` as "match any characters including '/'".
        /// </summary>
        /// <param name="pattern">The glob pattern to convert; may contain `*` and `**` wildcards.</param>
        /// <returns>A regex string fragment representing the provided glob pattern.</returns>
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
