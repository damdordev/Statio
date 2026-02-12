using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Damdor.VisualStates.Editor
{
    [CustomPropertyDrawer(typeof(VisualStatesList))]
    public class VisualStatesListPropertyDrawer : PropertyDrawer
    {
        private readonly Dictionary<string, ReorderableList> propertyPathToReorderableList = new();
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var list = GetReorderableList(property);
            return list.GetHeight();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var list = GetReorderableList(property);
            
            EditorGUI.BeginProperty(position, label, property);
            list.DoList(position);
            EditorGUI.EndProperty();
        }

        private ReorderableList GetReorderableList(SerializedProperty property)
        {
            property = property.FindPropertyRelative("states");
            var path = property.propertyPath;
            if(propertyPathToReorderableList.TryGetValue(path, out var reorderableList)) return reorderableList;

            reorderableList = new ReorderableList(
                property.serializedObject,
                property,
                true,
                true,
                true,
                true
            );

            reorderableList.drawHeaderCallback += rect =>
            {
                EditorGUI.LabelField(rect, "States");
            };

            reorderableList.drawElementCallback += (rect, index, _, _) =>
            {
                var elementProperty = property.GetArrayElementAtIndex(index);
                var oldValue = elementProperty.stringValue;
                var newValue = EditorGUI.DelayedTextField(rect, oldValue);
                if (oldValue == newValue) return;
                
                if (!HasState(property, newValue))
                {
                    elementProperty.stringValue = newValue;
                    property.serializedObject.ApplyModifiedProperties();
                }
            };

            reorderableList.elementHeightCallback += index
                => EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index));

            reorderableList.onAddDropdownCallback += (rect, _) =>
            {
                var newIndex = property.arraySize;
                property.InsertArrayElementAtIndex(newIndex);
                property.GetArrayElementAtIndex(newIndex).stringValue = GetNewStateName(property);
                property.serializedObject.ApplyModifiedProperties();
            };

            reorderableList.onReorderCallbackWithDetails += (l, oldIndex, newIndex) =>
            {
                Debug.LogError($"{oldIndex} > {newIndex}");
            };
            
            propertyPathToReorderableList.Add(path, reorderableList);

            return reorderableList;

        }

        private static string GetNewStateName(SerializedProperty property)
        {
            var name = 1;
            while (HasState(property, name.ToString())) ++name;
            return name.ToString();
        }

        private static bool HasState(SerializedProperty property, string state)
        {
            for (var i = 0; i < property.arraySize; i++)
            {
                if (property.GetArrayElementAtIndex(i).stringValue == state) return true;
            }

            return false;
        }
        
    }
}