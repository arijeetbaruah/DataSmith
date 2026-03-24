using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Baruah.DataSmith.Editor
{
    public class DataSmithGeneratorWindow : EditorWindow
    {
        private const string ConfigPrefKey = "DataSmith.GameModelConfigGUID";
        
        [SerializeField]
        private DataSmithConfig _config;

        private static List<ModelEntry> _entries = new();

        private static Vector2 _scroll;
        
        private GUIStyle _wrapLabel;
        private GUIStyle _headerLabel;
        private GUIStyle _rowEven;
        private GUIStyle _rowOdd;

        private GUIStyle RowEven
        {
            get
            {
                if (_rowEven == null)
                {
                    _rowEven = new GUIStyle(EditorStyles.helpBox);
                }
                
                return _rowEven;
            }
        }
        
        private GUIStyle RowOdd
        {
            get
            {
                if (_rowOdd == null)
                {
                    _rowOdd = new GUIStyle(EditorStyles.helpBox);
                    _rowOdd.normal.background = MakeTex(1, 1, new Color(0, 0, 0, 0.12f));
                }
                
                return _rowOdd;
            }
        }
        
        private GUIStyle WrapLabel
        {
            get
            {
                if (_wrapLabel == null)
                {
                    _wrapLabel = new GUIStyle(EditorStyles.label)
                    {
                        wordWrap = true,
                        alignment = TextAnchor.UpperLeft
                    };
                }

                return _wrapLabel;
            }
        }
        
        private GUIStyle HeaderLabel
        {
            get
            {
                if (_headerLabel == null)
                {
                    _headerLabel = new GUIStyle(EditorStyles.boldLabel)
                    {
                        alignment = TextAnchor.MiddleLeft
                    };
                }
                return _headerLabel;
            }
        }
        
        // =========================================================
        // OPEN WINDOW
        /// <summary>
        /// Opens the Game Model Generator editor window.
        /// </summary>

        [MenuItem("Tools/Game Model Generator")]
        public static void ShowWindow()
        {
            GetWindow<DataSmithGeneratorWindow>(
                "Game Model Generator");
        }

        /// <summary>
        /// Initialize the window's configuration and scan state when the editor window is enabled.
        /// </summary>
        /// <remarks>
        /// Loads a previously saved config reference (if any), selects a fallback config when none is saved,
        /// and performs a rescan to populate the model entries when a valid config is available.
        /// </remarks>
        private void OnEnable()
        {
            LoadSavedConfig();

            if (_config == null)
                _config = FindFallbackConfig();

            if (_config != null)
                Rescan();
        }

        // =========================================================
        // UI
        /// <summary>
        /// Renders the editor window UI for the DataSmith generator.
        /// </summary>
        /// <remarks>
        /// Draws the configuration selector; if no config is assigned, displays a warning and stops further rendering. When a config is present, renders the top bar and the results list.
        /// </remarks>

        private void OnGUI()
        {
            DrawConfigField();
            if (_config == null)
            {
                EditorGUILayout.HelpBox(
                    "Assign a GameModelConfig to use the generator.",
                    MessageType.Warning);
                return;
            }
            
            DrawTopBar();

            GUILayout.Space(10);

            DrawResults();
        }
        
        /// <summary>
        /// Creates a new Texture2D of the specified size filled entirely with the given color.
        /// </summary>
        /// <param name="w">Texture width in pixels.</param>
        /// <param name="h">Texture height in pixels.</param>
        /// <param name="col">Color to fill every pixel with.</param>
        /// <returns>A Texture2D of size <paramref name="w"/>×<paramref name="h"/> where every pixel is <paramref name="col"/>.</returns>
        private Texture2D MakeTex(int w, int h, Color col)
        {
            var tex = new Texture2D(w, h);
            var pixels = new Color[w * h];

            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = col;

            tex.SetPixels(pixels);
            tex.Apply();

            return tex;
        }

        // =========================================================
        // TOP BAR
        /// <summary>
        /// Renders the top toolbar with actions for operating on the current scan results.
        /// </summary>
        /// <remarks>
        /// The "Generate All" button starts generation for all currently scanned entries using the active configuration's output folder.
        /// The "Rescan" button refreshes the scan results.
        /// </remarks>

        private void DrawTopBar()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate All", GUILayout.Height(28)))
            {
                DataSmithGenerator.GenerateAll(_entries, _config.OutputFolder);
            }

            if (GUILayout.Button("Rescan", GUILayout.Width(80)))
            {
                Rescan();
            }

            EditorGUILayout.EndHorizontal();
        }

        // =========================================================
        // RESULTS TABLE
        /// <summary>
        /// Renders the "Scripts To Generate" panel: displays the entry count and a scrollable table of model entries with their columns and action buttons.
        /// </summary>
        /// <remarks>
        /// Computes column widths from the current window width, draws the table header, and iterates the cached entries to render each row inside a scroll view.
        /// </remarks>

        private void DrawResults()
        {
            GUILayout.Label($"Scripts To Generate ({_entries.Count})", EditorStyles.boldLabel);


            float totalWidth = position.width - 30f;
            
            float nameWidth = totalWidth * 0.10f;
            //float pathWidth = totalWidth * 0.45f;
            float nsWidth   = totalWidth * 0.80f;
            float btnWidth  = totalWidth * 0.10f;
            
            DrawHeader(nameWidth, nsWidth, btnWidth);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            int index = 0;
            foreach (var entry in _entries)
            {
                DrawEntryRow(entry, index, nameWidth, nsWidth, btnWidth);
                index++;
            }

            EditorGUILayout.EndScrollView();
        }
        
        /// <summary>
        /// Draws the table header row for the results list with column labels for Type, Namespace, and the action column.
        /// </summary>
        /// <param name="nameWidth">Pixel width of the "Type" column.</param>
        /// <param name="nsWidth">Pixel width of the "Namespace" column.</param>
        /// <param name="btnWidth">Pixel width of the rightmost action column (Generate button area).</param>
        private void DrawHeader(float nameWidth, float nsWidth, float btnWidth)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            GUILayout.Label("Type", HeaderLabel, GUILayout.Width(nameWidth));
            //GUILayout.Label("Path", HeaderLabel, GUILayout.Width(pathWidth));
            GUILayout.Label("Namespace", HeaderLabel, GUILayout.Width(nsWidth));
            GUILayout.Label("", GUILayout.Width(btnWidth));

            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Renders a single table row for a model entry showing its type name, namespace, and a "Generate" action.
        /// </summary>
        /// <param name="entry">The model entry to display.</param>
        /// <param name="index">Row index used to alternate row styling.</param>
        /// <param name="nameWidth">Column width allocated for the type name.</param>
        /// <param name="nsWidth">Column width allocated for the namespace (used to compute wrapped height).</param>
        /// <param name="btnWidth">Column width allocated for the action button.</param>
        private void DrawEntryRow(ModelEntry entry, int index, float nameWidth, float nsWidth, float btnWidth)
        {
            float singleLine = EditorGUIUtility.singleLineHeight;
            float pathHeight = WrapLabel.CalcHeight(new GUIContent(entry.Path ?? ""), nsWidth);
            float rowHeight = Mathf.Clamp(pathHeight, singleLine, singleLine * 2f);
            
            var style = (index % 2 == 0) ? RowEven : RowOdd;
            
            EditorGUILayout.BeginHorizontal(style, GUILayout.Height(rowHeight));

            GUILayout.Label(entry.Type.Name, WrapLabel, GUILayout.Width(nameWidth));
            //GUILayout.Label(entry.Path ?? "", WrapLabel, GUILayout.Width(pathWidth));
            GUILayout.Label(entry.Type.Namespace ?? "", WrapLabel, GUILayout.Width(nsWidth));

            if (GUILayout.Button("Generate", GUILayout.Width(btnWidth), GUILayout.Height(rowHeight)))
            {
                DataSmithGenerator.GenerateEntry(entry, _config.OutputFolder);
            }

            EditorGUILayout.EndHorizontal();
        }

        // =========================================================
        // PIPELINE
        /// <summary>
        /// Recomputes the list of model entries from the current config include/exclude paths and updates the window.
        /// </summary>
        /// <remarks>
        /// This replaces the cached entries with the latest scan results and schedules the editor window to repaint.
        /// </remarks>

        private void Rescan()
        {
            _entries = DataSmithScanner.Scan(_config.IncludePaths, _config.ExcludePaths);

            Repaint();
        }
        
        /// <summary>
        /// Renders the DataSmith configuration object field and updates the window state when the selection changes.
        /// </summary>
        /// <remarks>
        /// If the user changes the selected <see cref="DataSmithConfig"/>, the new reference is persisted and the entries list is rescanned to reflect the updated configuration.
        /// </remarks>
        private void DrawConfigField()
        {
            EditorGUI.BeginChangeCheck();

            _config = (DataSmithConfig)EditorGUILayout.ObjectField(
                "Config",
                _config,
                typeof(DataSmithConfig),
                false);

            if (EditorGUI.EndChangeCheck())
            {
                SaveConfigReference();
                Rescan();
            }
        }
        
        /// <summary>
        /// Persist the currently selected DataSmithConfig reference by storing its asset GUID in EditorPrefs, or remove the saved reference if none is selected.
        /// </summary>
        private void SaveConfigReference()
        {
            if (_config == null)
            {
                EditorPrefs.DeleteKey(ConfigPrefKey);
                return;
            }

            string path = AssetDatabase.GetAssetPath(_config);
            string guid = AssetDatabase.AssetPathToGUID(path);

            EditorPrefs.SetString(ConfigPrefKey, guid);
        }
        
        /// <summary>
        /// Restores the previously selected DataSmithConfig from EditorPrefs and assigns it to the window's _config field.
        /// </summary>
        /// <remarks>
        /// If no saved config GUID exists in EditorPrefs under the configured key, the method leaves _config unchanged.
        /// </remarks>
        private void LoadSavedConfig()
        {
            if (!EditorPrefs.HasKey(ConfigPrefKey))
                return;

            string guid = EditorPrefs.GetString(ConfigPrefKey);

            string path = AssetDatabase.GUIDToAssetPath(guid);

            _config = AssetDatabase.LoadAssetAtPath<DataSmithConfig>(path);
        }
        
        /// <summary>
        /// Locates a DataSmithConfig asset in the project, preferring assets under "Assets/" and then "Packages/".
        /// </summary>
        /// <summary>
        /// Chooses a fallback DataSmithConfig asset from the project for use when no configuration is selected.
        /// Prefers the first config found under the Assets folder; if none are found there, returns the first found under Packages.
        /// </summary>
        /// <returns>The first DataSmithConfig located under Assets/, or if none exists there the first under Packages/; returns null if no matching asset is found.</returns>
        private DataSmithConfig FindFallbackConfig()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(DataSmithConfig)}");

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.StartsWith("Assets/"))
                {
                    return AssetDatabase.LoadAssetAtPath<DataSmithConfig>(path);
                }
            }

            // fallback to package config
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.StartsWith("Packages/"))
                {
                    return AssetDatabase.LoadAssetAtPath<DataSmithConfig>(path);
                }
            }

            return null;
        }
    }
}
