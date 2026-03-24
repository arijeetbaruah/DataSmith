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
        /// Opens the Game Model Generator editor window and focuses it.
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
        /// Generates code artifacts for all scanned model entries using the currently selected configuration's output folder.
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
        /// Rebuilds the cached list of model entries from the current config and associates each entry with that config.
        /// </summary>
        /// <remarks>
        /// If no config is set, the method does nothing.
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
        /// Opens the Game Model Generator editor window and sets its title to "Game Model Generator".
        /// </summary>

        [MenuItem("Tools/Game Model Generator")]
        public static void ShowWindow()
        {
            GetWindow<DataSmithGeneratorWindow>(
                "Game Model Generator");
        }
        
        /// <summary>
        /// Initialize the editor window state by loading a previously saved configuration, using a fallback config if none was saved, and triggering a rescan when a configuration is available.
        /// </summary>
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
        /// Persist the current configuration asset reference and update the scanned model entries to reflect the change.
        /// </summary>
        /// <remarks>
        /// Saves or clears the stored config reference; if a config is set triggers a rescan, otherwise clears the entries list.
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
        /// Persist the currently selected DataSmith config asset reference in EditorPrefs or remove it if none is selected.
        /// </summary>
        /// <remarks>
        /// If `_config` is null, the stored preference identified by `ConfigPrefKey` is deleted. Otherwise the asset path of `_config` is converted to its GUID and stored under `ConfigPrefKey`.
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
        /// Restores the previously selected DataSmithConfig asset reference from EditorPrefs and assigns it to the window's `_config` field if present.
        /// </summary>
        private void LoadSavedConfig()
        {
            if (!EditorPrefs.HasKey(ConfigPrefKey))
                return;

            string guid = EditorPrefs.GetString(ConfigPrefKey);
            string path = AssetDatabase.GUIDToAssetPath(guid);

            _config = AssetDatabase.LoadAssetAtPath<DataSmithConfig>(path);
        }
        
        /// <summary>
        /// Locates a DataSmithConfig asset in the project, preferring assets under "Assets/" and falling back to assets under "Packages/".
        /// </summary>
        /// <returns>`DataSmithConfig` instance found in the project, or `null` if no matching asset exists.</returns>
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
