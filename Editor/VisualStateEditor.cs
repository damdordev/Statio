using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Damdor.VisualStates.Editor
{
    [CustomEditor(typeof(VisualState), true)]
    public class VisualStateEditor : UnityEditor.Editor
    {
        private const float GoToStateButtonWidth = 50f;
        
        private readonly Dictionary<string, ReorderableList> propertyPathToReorderableList = new();
        private VisualState state;
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            state = serializedObject.targetObject as VisualState;
            var statesProperty = serializedObject.FindProperty("states");
            var storageProperty = serializedObject.FindProperty("storage");
            var parametersProperty = serializedObject.FindProperty("parameters");
            var initialStateProperty = serializedObject.FindProperty("initialStateId");

            if(!propertyPathToReorderableList.TryGetValue(statesProperty.propertyPath, out var statesList))
            {
                statesList = CreateStatesReorderableList(statesProperty);
            }

            if (!propertyPathToReorderableList.TryGetValue(parametersProperty.propertyPath, out var parametersList))
            {
                parametersList = CreateParametersReorderableList(parametersProperty);
            }

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
                EditorGUI.LabelField( rect, "States");
            };

            reorderableList.drawElementCallback += (rect, index, _, _) =>
            {
                var elementProperty = property.GetArrayElementAtIndex(index);
                var buttonClicked = GUI.Button(
                    new Rect(rect.x + rect.width - GoToStateButtonWidth, rect.y, GoToStateButtonWidth, rect.height),
                    state.CurrentState == elementProperty.stringValue ? "x" : ""
                );
                if (buttonClicked)
                {
                    state.ChangeState(elementProperty.stringValue);
                }
                
                var oldValue = elementProperty.stringValue;
                var newValue = EditorGUI.DelayedTextField(
                    new Rect(rect.x, rect.y, rect.width - GoToStateButtonWidth, rect.height),
                    oldValue
                );
                if (oldValue == newValue) return;
                
                if (!VisualStateEditorHelper.HasState(property, newValue))
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
                property.GetArrayElementAtIndex(newIndex).stringValue = VisualStateEditorHelper.GetNewStateName(property);
                property.serializedObject.ApplyModifiedProperties();
            };

            reorderableList.onReorderCallbackWithDetails += (l, oldIndex, newIndex) =>
            {
                Undo.RecordObject(property.serializedObject.targetObject, $"Reorder state");
                state.ChangeStateIndex(state.States[oldIndex], newIndex);
                EditorUtility.SetDirty(property.serializedObject.targetObject);
                property.serializedObject.Update();
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.ApplyModifiedProperties();
            };

            reorderableList.onRemoveCallback += l =>
            {
                Undo.RecordObject(property.serializedObject.targetObject, $"Remove state");
                state.RemoveState(state.States[l.index]);
                EditorUtility.SetDirty(property.serializedObject.targetObject);
                property.serializedObject.Update();
                property.serializedObject.ApplyModifiedProperties();
            };

            propertyPathToReorderableList[property.propertyPath] = reorderableList;
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
                EditorGUI.PropertyField(
                    new Rect(rect.x + 10f, rect.y, rect.width - 10f, rect.height),
                    property.GetArrayElementAtIndex(index),
                    true
                );
            };
            
            reorderableList.elementHeightCallback += index 
                => EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index));

            reorderableList.onAddDropdownCallback += (rect, _) =>
            {
                var types = VisualStateSettings.SupportedParameterTypes.OrderBy(VisualStateEditorHelper.GetParameterTypeName).ToList();
                var variables = types.Select(VisualStateEditorHelper.GetParameterTypeName).ToArray();
                new StringDropdown(variables, newChoice =>
                {
                    property.InsertArrayElementAtIndex(property.arraySize);
                    var element = property.GetArrayElementAtIndex(property.arraySize - 1);
                    var index = Array.FindIndex(variables, v => v == newChoice);
                    var type = types[index];
                    element.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                }).Show(new Rect(rect.x - 300, rect.y, rect.width + 300, rect.height));
            };
            
            propertyPathToReorderableList[property.propertyPath] = reorderableList;
            
            return reorderableList;
        }
        
    }
}