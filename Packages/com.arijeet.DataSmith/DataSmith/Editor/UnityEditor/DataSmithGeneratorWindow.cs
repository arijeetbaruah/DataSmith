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

        [MenuItem("Tools/Game Model Generator")]
        public static void ShowWindow()
        {
            GetWindow<DataSmithGeneratorWindow>("Game Model Generator");
        }

        // =========================================================
        // UI TOOLKIT ENTRY POINT
        // =========================================================

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
