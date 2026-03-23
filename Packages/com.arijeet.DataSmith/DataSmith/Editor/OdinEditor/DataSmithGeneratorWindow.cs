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
        /// <summary>
        /// Opens the Game Model Generator editor window.
        /// </summary>
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
        /// <summary>
        /// Triggers generation of all model scripts using the current configuration.
        /// </summary>
        /// <remarks>
        /// Does nothing if no configuration is selected.
        /// </remarks>

        [PropertySpace(10)]
        [HorizontalGroup("Toolbar")]
        [Button(ButtonSizes.Large)]
        private void GenerateAll()
        {
            if (_config == null) return;
            DataSmithGenerator.GenerateAll(_entries, _config.OutputFolder);
        }

        /// <summary>
        /// Refreshes the list of model entries from the active configuration and applies that configuration to each entry.
        /// </summary>
        /// <remarks>
        /// If no configuration is selected, the method does nothing.
        /// </remarks>
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
        /// <summary>
        /// Opens or focuses the "Game Model Generator" editor window.
        /// </summary>

        [MenuItem("Tools/Game Model Generator")]
        public static void ShowWindow()
        {
            GetWindow<DataSmithGeneratorWindow>(
                "Game Model Generator");
        }
        
        /// <summary>
        /// Initialize the editor window by restoring a previously saved DataSmithConfig, picking a fallback config if none is saved, and rescanning entries when a config is available.
        /// </summary>
        /// <remarks>
        /// Called when the Unity editor enables the window; invokes base.OnEnable(), attempts to load a saved config, falls back to a discovered config if needed, and calls Rescan() when a config is set.
        /// </remarks>
        protected override void OnEnable()
        {
            base.OnEnable();

            LoadSavedConfig();

            if (_config == null)
                _config = FindFallbackConfig();

            if (_config != null)
                Rescan();
        }
        
        /// <summary>
        /// Handle actions required when the selected configuration asset changes.
        /// </summary>
        /// <remarks>
        /// Persists the new configuration reference (or removes the stored reference if null). If a configuration is set, triggers a rescan of model entries; if the configuration is cleared, empties the current entry list.
        /// </remarks>
        private void OnConfigChanged()
        {
            SaveConfigReference();

            if (_config != null)
                Rescan();
            else
                _entries.Clear();
        }

        /// <summary>
        /// Persists the currently selected DataSmithConfig reference to EditorPrefs or removes the saved reference if none is selected.
        /// </summary>
        /// <remarks>
        /// If <c>_config</c> is null, the stored config GUID under <c>ConfigPrefKey</c> is deleted. Otherwise the asset path of <c>_config</c> is converted to a GUID and saved to <c>ConfigPrefKey</c>.
        /// </remarks>
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
        /// Loads the persisted DataSmithConfig reference from EditorPrefs and assigns it to the window's _config field.
        /// </summary>
        /// <remarks>
        /// If the editor preference key is not present this method does nothing. If the stored GUID does not resolve to a valid asset path or the asset cannot be loaded, _config will be set to null.
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
        /// Locate a DataSmithConfig asset in the project, preferring assets under "Assets/" and then "Packages/".
        /// </summary>
        /// <returns>The first matching DataSmithConfig found, or null if no GameModelConfig asset exists.</returns>
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
