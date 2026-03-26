using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Baruah.DataSmith.Editor
{
    public class DataSmithGeneratorWindow : EditorWindow
    {
        private const string ConfigPrefKey = "DataSmith.GameModelConfigGUID";

        private DataSmithConfig _config;
        private List<ModelEntry> _entries = new();

        private ListView _listView;
        private Label _countLabel;

        /// <summary>
        /// Opens the Game Model Generator editor window.
        /// </summary>
        /// <remarks>Registered as a menu command under Tools/Game Model Generator.</remarks>
        [MenuItem("Tools/Game Model Generator")]
        public static void ShowWindow()
        {
            GetWindow<DataSmithGeneratorWindow>("Game Model Generator");
        }

        // =========================================================
        // UI TOOLKIT ENTRY POINT
        // =========================================================
        
        /// <summary>
        /// Builds and initializes the UI Toolkit layout for the editor window, including the config selector, top toolbar, count label, and the list of model entries.
        /// </summary>
        /// <remarks>
        /// Loads a saved config or finds a fallback; if no config is available, displays a warning and stops further UI construction.  
        /// The config ObjectField updates the stored config, persists the reference, and triggers a rescan when changed.  
        /// The toolbar provides "Generate All" and "Rescan" actions. The ListView is configured with item creation and binding callbacks and a final rescan is invoked to populate the view.
        /// </remarks>

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.paddingLeft = 6;
            root.style.paddingRight = 6;
            root.style.paddingTop = 6;

            LoadSavedConfig();
            if (_config == null)
                _config = FindFallbackConfig();

            // -----------------------------------------------------
            // Config Field
            // -----------------------------------------------------

            var configField = new ObjectField("Config")
            {
                objectType = typeof(DataSmithConfig),
                value = _config
            };

            configField.RegisterValueChangedCallback(evt =>
            {
                _config = (DataSmithConfig)evt.newValue;
                SaveConfigReference();
                Rescan();
            });

            root.Add(configField);

            // Warning box if no config
            if (_config == null)
            {
                root.Add(new HelpBox(
                    "Assign a GameModelConfig to use the generator.",
                    HelpBoxMessageType.Warning));
                return;
            }

            // -----------------------------------------------------
            // Top Bar
            // -----------------------------------------------------

            var toolbar = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    marginTop = 6,
                    marginBottom = 6
                }
            };

            var generateAllBtn = new Button(() =>
            {
                DataSmithGenerator.GenerateAll(_entries, _config.OutputFolder);
            })
            { text = "Generate All" };

            generateAllBtn.style.height = 28;

            var rescanBtn = new Button(Rescan)
            { text = "Rescan" };

            toolbar.Add(generateAllBtn);
            toolbar.Add(rescanBtn);

            root.Add(toolbar);

            // -----------------------------------------------------
            // Header
            // -----------------------------------------------------

            _countLabel = new Label();
            _countLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            root.Add(_countLabel);

            // -----------------------------------------------------
            // ListView (TABLE)
            // -----------------------------------------------------

            _listView = new ListView
            {
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                selectionType = SelectionType.None,
                style = { flexGrow = 1 }
            };

            _listView.makeItem = MakeItem;
            _listView.bindItem = BindItem;

            root.Add(_listView);

            Rescan();
        }

        // =========================================================
        // LIST VIEW
        // =========================================================
        
        /// <summary>
        /// Creates a horizontal row used as a ListView item with a type label, a namespace label, and a Generate button.
        /// </summary>
        /// <returns>A VisualElement representing the row containing: a <c>Label</c> named &quot;type&quot; for the type name, a <c>Label</c> named &quot;ns&quot; for the namespace, and a <c>Button</c> named &quot;btn&quot; with the text &quot;Generate&quot;.</returns>

        private VisualElement MakeItem()
        {
            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    paddingLeft = 4,
                    paddingRight = 4,
                    paddingTop = 2,
                    paddingBottom = 2
                }
            };

            var typeLabel = new Label { name = "type" };
            typeLabel.style.width = 150;
            typeLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

            var nsLabel = new Label { name = "ns" };
            nsLabel.style.flexGrow = 1;
            nsLabel.style.whiteSpace = WhiteSpace.Normal;

            var button = new Button { name = "btn", text = "Generate" };
            button.style.width = 100;

            row.Add(typeLabel);
            row.Add(nsLabel);
            row.Add(button);

            return row;
        }

        /// <summary>
        /// Binds the model entry at the given index to the provided row visual element: populates labels, wires the Generate button, and applies alternating background color.
        /// </summary>
        /// <param name="element">The row VisualElement created by MakeItem to populate.</param>
        /// <param name="index">The index of the entry in the internal entries list to bind.</param>
        private void BindItem(VisualElement element, int index)
        {
            var entry = _entries[index];

            element.Q<Label>("type").text = entry.Type.Name;
            element.Q<Label>("ns").text = entry.Type.Namespace ?? "";

            var btn = element.Q<Button>("btn");
            btn.clicked += () =>
            {
                DataSmithGenerator.GenerateEntry(entry, _config.OutputFolder);
            };

            // Alternating row color
            element.style.backgroundColor =
                index % 2 == 0
                    ? new Color(0, 0, 0, 0)
                    : new Color(0, 0, 0, 0.12f);
        }

        // =========================================================
        // PIPELINE
        // =========================================================
        
        /// <summary>
        /// Updates the cached model entries by scanning using the current config and refreshes the ListView and count label.
        /// </summary>
        /// <remarks>
        /// Does nothing if no config is selected.
        /// </remarks>

        private void Rescan()
        {
            if (_config == null)
                return;

            _entries = DataSmithScanner.Scan(
                _config.IncludePaths,
                _config.ExcludePaths);

            _listView.itemsSource = _entries;
            _listView.Rebuild();

            _countLabel.text = $"Scripts To Generate ({_entries.Count})";
        }

        // =========================================================
        // CONFIG PERSISTENCE
        // =========================================================
        
        /// <summary>
        /// Persist the current DataSmithConfig reference by storing its asset GUID in EditorPrefs.
        /// </summary>
        /// <remarks>
        /// If <c>_config</c> is <c>null</c>, the stored preference under <c>ConfigPrefKey</c> is removed; otherwise the asset's GUID is saved under that key.
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
        /// Loads the previously saved DataSmithConfig reference from EditorPrefs into the window's _config field.
        /// </summary>
        /// <remarks>
        /// If the EditorPrefs key defined by ConfigPrefKey is not present, this method leaves _config unchanged.
        /// When the key exists, it reads the stored GUID, resolves it to an asset path, and attempts to load the DataSmithConfig asset;
        /// _config will be set to the loaded asset or null if the asset cannot be found.
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
        /// Finds a DataSmithConfig asset in the project, preferring assets under "Assets/" and falling back to those under "Packages/".
        /// </summary>
        /// <returns>The first matching DataSmithConfig found in "Assets/", or if none is found, the first matching config in "Packages/"; returns <c>null</c> if no config exists.</returns>
        private DataSmithConfig FindFallbackConfig()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(DataSmithConfig)}");

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
