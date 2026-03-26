using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Baruah.DataSmith.Database.Editor
{
    [CustomPropertyDrawer(typeof(DatabaseProviderAsset), true)]
    public class DatabaseProviderAssetDrawer : PropertyDrawer
    {
        private static List<Type> _types;

        private static List<Type> GetTypes()
        {
            if (_types != null) return _types;

            _types = TypeCache.GetTypesDerivedFrom<DatabaseProviderAsset>()
                .Where(t => !t.IsAbstract && !t.IsGenericType)
                .OrderBy(t => t.Name)
                .ToList();

            return _types;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight; // label
            height += EditorGUIUtility.singleLineHeight
                      + EditorGUIUtility.standardVerticalSpacing; // dropdown

            if (!property.isExpanded || property.managedReferenceValue == null)
                return height;

            var copy = property.Copy();
            var end = copy.GetEndProperty();

            copy.NextVisible(true);
            while (!SerializedProperty.EqualContents(copy, end))
            {
                height += EditorGUI.GetPropertyHeight(copy, true)
                          + EditorGUIUtility.standardVerticalSpacing;

                if (!copy.NextVisible(false))
                    break;
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var types = GetTypes();

            float line = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            Rect labelRect = new Rect(position.x, position.y, position.width, line);
            EditorGUI.LabelField(labelRect, label);

            // ---------- TYPE DROPDOWN (LINE 2) ----------
            Rect dropdownRect = new Rect(
                position.x,
                position.y + line + spacing,
                position.width,
                line
            );

            string typeName = property.managedReferenceValue == null
                ? "Select Type"
                : property.managedReferenceValue.GetType().Name;

            if (GUI.Button(dropdownRect, typeName, EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(
                    new GUIContent("Select Provider"),
                    property.managedReferenceValue == null,
                    () =>
                    {
                        property.serializedObject.Update();
                        property.managedReferenceValue = null;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                );
                
                foreach (var t in types)
                {
                    menu.AddItem(
                        new GUIContent(t.Name),
                        property.managedReferenceValue?.GetType() == t,
                        () =>
                        {
                            property.serializedObject.Update();
                            property.managedReferenceValue = Activator.CreateInstance(t);
                            property.isExpanded = true;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                }

                menu.ShowAsContext();
            }

            // ---------- CHILD FIELDS ----------
            if (property.isExpanded && property.managedReferenceValue != null)
            {
                EditorGUI.indentLevel++;

                Rect childRect = new Rect(
                    position.x,
                    dropdownRect.yMax + spacing,
                    position.width,
                    line
                );

                var copy = property.Copy();
                var end = copy.GetEndProperty();

                copy.NextVisible(true);
                while (!SerializedProperty.EqualContents(copy, end))
                {
                    float h = EditorGUI.GetPropertyHeight(copy, true);

                    childRect.height = h;
                    EditorGUI.PropertyField(childRect, copy, true);

                    childRect.y += h + spacing;

                    if (!copy.NextVisible(false))
                        break;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
    }
}
