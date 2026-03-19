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
        // =========================================================

        [MenuItem("Tools/Game Model Generator")]
        public static void ShowWindow()
        {
            GetWindow<DataSmithGeneratorWindow>(
                "Game Model Generator");
        }

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
        // =========================================================

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
        // =========================================================

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
        // =========================================================

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
        
        private void DrawHeader(float nameWidth, float nsWidth, float btnWidth)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            GUILayout.Label("Type", HeaderLabel, GUILayout.Width(nameWidth));
            //GUILayout.Label("Path", HeaderLabel, GUILayout.Width(pathWidth));
            GUILayout.Label("Namespace", HeaderLabel, GUILayout.Width(nsWidth));
            GUILayout.Label("", GUILayout.Width(btnWidth));

            EditorGUILayout.EndHorizontal();
        }
        
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
        // =========================================================

        private void Rescan()
        {
            _entries = DataSmithScanner.Scan(_config.IncludePaths, _config.ExcludePaths);

            Repaint();
        }
        
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
        
        private void LoadSavedConfig()
        {
            if (!EditorPrefs.HasKey(ConfigPrefKey))
                return;

            string guid = EditorPrefs.GetString(ConfigPrefKey);

            string path = AssetDatabase.GUIDToAssetPath(guid);

            _config = AssetDatabase.LoadAssetAtPath<DataSmithConfig>(path);
        }
        
        private DataSmithConfig FindFallbackConfig()
        {
            var guids = AssetDatabase.FindAssets("t:GameModelConfig");

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
