using System.IO;
using UnityEngine;
using UnityEditor;

namespace Baruah.DataSmith.Editor
{
    [CustomEditor(typeof(DataSmithConfig))]
    [CanEditMultipleObjects]
    public class DataSmithConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty _outputFolder;
        private SerializedProperty _includePaths;
        private SerializedProperty _excludePaths;
        
        /// <summary>
        /// Initializes and caches the serialized properties used by this custom inspector: `_outputFolder`, `_includePaths`, and `_excludePaths`.
        /// </summary>
        private void OnEnable()
        {
            _outputFolder = serializedObject.FindProperty("_outputFolder");
            _includePaths = serializedObject.FindProperty("_includePaths");
            _excludePaths = serializedObject.FindProperty("_excludePaths");
        }
        
        /// <summary>
        /// Renders the custom inspector UI for DataSmithConfig, including the output-folder controls, include/exclude path fields, and an "Open Window" button.
        /// </summary>
        /// <remarks>
        /// Synchronizes serialized state before drawing and applies any modified properties after drawing.
        /// Clicking the "Open Window" button opens the DataSmith generator window.
        /// </remarks>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawOutputFolder();
            EditorGUILayout.Space(8);

            EditorGUILayout.PropertyField(_includePaths);
            EditorGUILayout.Space(6);
            EditorGUILayout.PropertyField(_excludePaths);

            if (GUILayout.Button("Open Window"))
            {
                DataSmithGeneratorWindow.ShowWindow();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        /// <summary>
        /// Renders the "Output Folder" inspector UI: a property field with a "Browse" button, validation, and a folder creation control.
        /// </summary>
        /// <remarks>
        /// The "Browse" button opens a folder picker and, if a folder inside the project's Assets path is selected, stores the path as a project-relative "Assets/..." value. If a selection is outside the project, an "Invalid Folder" dialog is shown. If the configured folder does not exist, a warning is displayed and a "Create Folder" button is provided to create the folder in the project.
        /// </remarks>
        private void DrawOutputFolder()
        {
            EditorGUILayout.LabelField("Output Folder", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(_outputFolder, GUIContent.none);

            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string start = string.IsNullOrEmpty(_outputFolder.stringValue)
                    ? Application.dataPath
                    : Path.Combine(Application.dataPath, _outputFolder.stringValue);

                string path = EditorUtility.OpenFolderPanel("Select Output Folder", start, "");

                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith(Application.dataPath))
                    {
                        _outputFolder.stringValue = "Assets" + path.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Invalid Folder", "Please select a folder inside the project (Assets).", "OK");
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            if (!AssetDatabase.IsValidFolder(_outputFolder.stringValue))
            {
                EditorGUILayout.HelpBox("Folder does not exist.", MessageType.Warning);

                if (GUILayout.Button("Create Folder"))
                {
                    CreateFolder(_outputFolder.stringValue);
                }
            }
        }
        
        /// <summary>
        /// Ensures the given Unity asset folder exists by creating the folder and any missing parent folders in the AssetDatabase.
        /// </summary>
        /// <param name="assetPath">Project-relative folder path (for example "Assets/Textures/SubFolder"). If null or empty, no action is taken.</param>
        private void CreateFolder(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
                return;

            string parent = System.IO.Path.GetDirectoryName(assetPath);
            string name = System.IO.Path.GetFileName(assetPath);

            if (!AssetDatabase.IsValidFolder(parent))
                CreateFolder(parent);

            AssetDatabase.CreateFolder(parent, name);
            AssetDatabase.Refresh();
        }
    }
}
