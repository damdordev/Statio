using System;
using System.Collections.Generic;
using System.Linq;
using Damdor.Vario.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Damdor.Statio.Editor
{
    /// <summary>
    /// Draws the VisualState Inspector for states, parameters, and animation rules.
    /// </summary>
    [CustomEditor(typeof(VisualState), true)]
    public class VisualStateEditor : UnityEditor.Editor
    {
        private const float GoToStateButtonWidth = 50f;
        
        private readonly Dictionary<string, ReorderableList> propertyPathToReorderableList = new();
        private VisualState visualState;
        private float lastTime;

        private void OnEnable()
        {
            lastTime = -1f;
            EditorApplication.update += EditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
        }

        /// <summary>
        /// Draws and applies the custom Inspector settings.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            visualState = serializedObject.targetObject as VisualState;
            var statesProperty = serializedObject.FindProperty("states");
            var storageProperty = serializedObject.FindProperty("storage");
            var parametersProperty = serializedObject.FindProperty("parameters");
            var animationsProperty = serializedObject.FindProperty("animations");
            var initialStateProperty = serializedObject.FindProperty("initialStateId");
            var timescaleProperty = serializedObject.FindProperty("timescale");

            if(!propertyPathToReorderableList.TryGetValue(statesProperty.propertyPath, out var statesList))
            {
                statesList = CreateStatesReorderableList(statesProperty);
            }
            
            if(!propertyPathToReorderableList.TryGetValue(animationsProperty.propertyPath, out var animationsList))
            {
                animationsList = CreateAnimationsList(animationsProperty);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Initial state", GUILayout.ExpandWidth(false), GUILayout.Width(100));

            StatioEditorHelper.ShowStateChoice(visualState, initialStateProperty.intValue, "-----", newStateId =>
            {
                initialStateProperty.intValue = newStateId;
                serializedObject.ApplyModifiedProperties();
            });
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(timescaleProperty);
            
            if (!propertyPathToReorderableList.TryGetValue(parametersProperty.propertyPath, out var parametersList))
            {
                parametersList = CreateParametersReorderableList(parametersProperty);
            }

            EditorGUILayout.BeginVertical();
            statesList.DoLayoutList();
            EditorGUILayout.PropertyField(storageProperty);
            parametersList.DoLayoutList();
            animationsList.DoLayoutList();
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
                    visualState.CurrentState == elementProperty.stringValue ? "x" : ""
                );
                if (buttonClicked)
                {
                    visualState.ChangeState(elementProperty.stringValue);
                }
                
                var oldValue = elementProperty.stringValue;
                var newValue = EditorGUI.DelayedTextField(
                    new Rect(rect.x, rect.y, rect.width - GoToStateButtonWidth, rect.height),
                    oldValue
                );
                if (oldValue == newValue) return;
                
                if (!StatioEditorHelper.HasState(property, newValue))
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
                property.GetArrayElementAtIndex(newIndex).stringValue = StatioEditorHelper.GetNewStateName(property);
                property.serializedObject.ApplyModifiedProperties();
            };

            reorderableList.onReorderCallbackWithDetails += (l, oldIndex, newIndex) =>
            {
                Undo.RecordObject(property.serializedObject.targetObject, $"Reorder state");
                visualState.ChangeStateId(visualState.States[oldIndex], newIndex);
                EditorUtility.SetDirty(property.serializedObject.targetObject);
                property.serializedObject.Update();
                property.serializedObject.ApplyModifiedProperties();
            };

            reorderableList.onRemoveCallback += l =>
            {
                Undo.RecordObject(property.serializedObject.targetObject, $"Remove state");
                visualState.RemoveState(visualState.States[l.index]);
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
                var types = StatioSettings.SupportedParameterTypes.OrderBy(StatioSettings.GetParameterTypeName).ToList();
                new HierarchicalDropdown<Type>(types, StatioSettings.GetParameterTypeName, type =>
                {
                    property.InsertArrayElementAtIndex(property.arraySize);
                    var element = property.GetArrayElementAtIndex(property.arraySize - 1);
                    element.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                }).Show(new Rect(rect.x - 300, rect.y, rect.width + 300, rect.height));
            };
            
            propertyPathToReorderableList[property.propertyPath] = reorderableList;
            
            return reorderableList;
        }

        private ReorderableList CreateAnimationsList(SerializedProperty property)
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
                EditorGUI.LabelField(rect, "Animations");
            };
            
            reorderableList.elementHeight += 3f * EditorGUIUtility.singleLineHeight + 2f * EditorGUIUtility.standardVerticalSpacing;
            
            reorderableList.drawElementCallback += (rect, index, _, _) =>
            {
                var initialStateIdProperty = property.GetArrayElementAtIndex(index).FindPropertyRelative("InitialStateId");
                var initialStateIdRect = new Rect(
                    rect.x,
                    rect.y,
                    rect.width / 2f - 10f,
                    EditorGUIUtility.singleLineHeight
                );
                StatioEditorHelper.ShowStateChoice(initialStateIdRect, visualState, initialStateIdProperty.intValue, "*", newStateId =>
                {
                    initialStateIdProperty.intValue = newStateId;
                    property.serializedObject.ApplyModifiedProperties();
                });
                
                var targetStateIdProperty = property.GetArrayElementAtIndex(index).FindPropertyRelative("TargetStateId");
                var targetStateIdRect = new Rect(
                    rect.x + rect.width/2f + 5f,
                    rect.y,
                    rect.width / 2f - 10f,
                    EditorGUIUtility.singleLineHeight
                );
                StatioEditorHelper.ShowStateChoice(targetStateIdRect, visualState, targetStateIdProperty.intValue, "*", newStateId =>
                {
                    targetStateIdProperty.intValue = newStateId;
                    property.serializedObject.ApplyModifiedProperties();
                });
                
                var durationProperty = property.GetArrayElementAtIndex(index).FindPropertyRelative("Duration");
                var durationRect = new Rect(
                    rect.x,
                    rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                    rect.width,
                    EditorGUIUtility.singleLineHeight
                );

                EditorGUI.PropertyField(durationRect, durationProperty);
                
                var easingProperty = property.GetArrayElementAtIndex(index).FindPropertyRelative("Easing");
                var easingRect = new Rect(
                    rect.x,
                    rect.y + 2f * EditorGUIUtility.singleLineHeight + 2f * EditorGUIUtility.standardVerticalSpacing,
                    rect.width,
                    EditorGUIUtility.singleLineHeight
                );

                EditorGUI.PropertyField(easingRect, easingProperty);
            };

            reorderableList.onAddDropdownCallback += (rect, _) =>
            {
                var index = property.arraySize;
                property.InsertArrayElementAtIndex(index);
                property.GetArrayElementAtIndex(index).FindPropertyRelative("InitialStateId").intValue = -1;
                property.GetArrayElementAtIndex(index).FindPropertyRelative("TargetStateId").intValue = -1;
            };
            
            propertyPathToReorderableList[property.propertyPath] = reorderableList;
            
            return reorderableList;
        }
        
        private void EditorUpdate()
        {
            if (Application.isPlaying) return;
            if (visualState == null || !visualState.isActiveAndEnabled) return;
            
            var time = Time.realtimeSinceStartup;
            if(lastTime > 0f)
            {
                var dt = time - lastTime;
                visualState.UpdateTime(dt);
            }

            lastTime = time;
        }
        
    }
}