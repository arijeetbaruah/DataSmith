using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Baruah.DataSmith.Editor
{
    public class DataSmithGeneratorWindow : OdinEditorWindow
    {
        private const string ConfigPrefKey = "DataSmith.GameModelConfigGUID";

        // =========================================================
        // MENU
        [MenuItem("Tools/Game Model Generator")]
        private static void Open()
        {
            GetWindow<DataSmithGeneratorWindow>("Game Model Generator");
        }

        // =========================================================
        // CONFIG

        [Title("Configuration")]
        [OnValueChanged(nameof(OnConfigChanged))]
        [SerializeField]
        private DataSmithConfig _config;

        // =========================================================
        // TOP ACTIONS

        [PropertySpace(10)]
        [HorizontalGroup("Toolbar")]
        [Button(ButtonSizes.Large)]
        private void GenerateAll()
        {
            if (_config == null) return;
            DataSmithGenerator.GenerateAll(_entries, _config.OutputFolder);
        }

        [HorizontalGroup("Toolbar", Width = 120)]
        [Button(ButtonSizes.Large)]
        private void Rescan()
        {
            if (_config == null) return;

            _entries = DataSmithScanner.Scan(_config.IncludePaths, _config.ExcludePaths);
            foreach (var entity in _entries)
            {
                entity.SetConfigFile(_config);
            }
        }

        // =========================================================
        // RESULTS TABLE

        [ShowIf(nameof(_config)), ShowInInspector]
        [TableList(AlwaysExpanded = true, ShowPaging = false, DrawScrollView = true, IsReadOnly = true)]
        [LabelText("Scripts To Generate")]
        private List<ModelEntry> _entries = new();
        
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
        
        protected override void OnEnable()
        {
            base.OnEnable();

            LoadSavedConfig();

            if (_config == null)
                _config = FindFallbackConfig();

            if (_config != null)
                Rescan();
        }
        
        private void OnConfigChanged()
        {
            SaveConfigReference();

            if (_config != null)
                Rescan();
            else
                _entries.Clear();
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
                    return AssetDatabase.LoadAssetAtPath<DataSmithConfig>(path);
            }

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.StartsWith("Packages/"))
                    return AssetDatabase.LoadAssetAtPath<DataSmithConfig>(path);
            }

            return null;
        }
    }
}
