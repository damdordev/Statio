using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Damdor.VisualStates.Editor
{
    [CustomEditor(typeof(VisualState), true)]
    public class VisualStateEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var statesProperty = serializedObject.FindProperty("states");
            var storageProperty = serializedObject.FindProperty("storage");
            var parametersProperty = serializedObject.FindProperty("parameters");

            var statesList = CreateStatesReorderableList(statesProperty);
            var parametersList = CreateParametersReorderableList(parametersProperty);
            
            EditorGUILayout.BeginVertical();
            statesList.DoLayoutList();
            EditorGUILayout.PropertyField(storageProperty);
            parametersList.DoLayoutList();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private ReorderableList CreateStatesReorderableList(SerializedProperty property)
        {
            var reorderableList = new ReorderableList(
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
            
            return reorderableList;
        }
        
        private ReorderableList CreateParametersReorderableList(SerializedProperty property)
        {
            var reorderableList = new ReorderableList(
                property.serializedObject,
                property,
                true,
                true,
                true,
                true
            );

            reorderableList.drawHeaderCallback += rect =>
            {
                EditorGUI.LabelField(rect, "Parameters");
            };

            reorderableList.drawElementCallback += (rect, index, _, _) =>
            {
                EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(index), true);
            };
            
            reorderableList.elementHeightCallback += index 
                => EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index));

            reorderableList.onAddDropdownCallback += (rect, _) =>
            {
                var newIndex = property.arraySize;
                property.InsertArrayElementAtIndex(newIndex);
                property.GetArrayElementAtIndex(newIndex).managedReferenceValue = new PositionVisualStateParameter();
                property.serializedObject.ApplyModifiedProperties();
            };
            
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