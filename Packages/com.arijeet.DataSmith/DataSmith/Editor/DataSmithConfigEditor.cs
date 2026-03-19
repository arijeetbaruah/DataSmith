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
        
        private void OnEnable()
        {
            _outputFolder = serializedObject.FindProperty("_outputFolder");
            _includePaths = serializedObject.FindProperty("_includePaths");
            _excludePaths = serializedObject.FindProperty("_excludePaths");
        }
        
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
