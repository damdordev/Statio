using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Damdor.VisualStates.Editor
{
    [CustomPropertyDrawer(typeof(VisualStateParameter))]
    public class VisualStateParameterPropertyDrawer : PropertyDrawer
    {
        private const string TargetPropertyName = "target";
        private const string DefaultValuePropertyName = "defaultValue";
        private const string ValuesPropertyName = "values";

        private const float CheckboxSize = 30f;
        
        private SerializedProperty targetProperty;
        private SerializedProperty defaultValueProperty;
        private SerializedProperty valuesProperty;
        private List<SerializedProperty> otherProperties = new();
        private VisualState state;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            RetrieveProperties(property);
            var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (property.isExpanded)
            {
                height += EditorGUI.GetPropertyHeight(defaultValueProperty);
                height += EditorGUIUtility.standardVerticalSpacing;

                for (var i = 0; i < valuesProperty.arraySize; ++i)
                {
                    height += Mathf.Max(
                        EditorGUI.GetPropertyHeight(valuesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("value")),
                        EditorGUIUtility.singleLineHeight
                    );
                    height += EditorGUIUtility.standardVerticalSpacing;
                }

                height += Mathf.Max(0, state.States.Count - valuesProperty.arraySize) *
                          (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                
                foreach (var childProperty in otherProperties)
                {
                    height += EditorGUI.GetPropertyHeight(childProperty);
                    height +=  EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RetrieveProperties(property);
            EditorGUI.BeginProperty(position, label, property);

            var y = position.y;
            ShowTopLine(
                new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
                property,
                label
            );
            y += EditorGUIUtility.singleLineHeight;
            y += EditorGUIUtility.standardVerticalSpacing;

            if (property.isExpanded)
            {
                ShowDefaultValue(
                    new Rect(position.x, y, position.width, EditorGUI.GetPropertyHeight(defaultValueProperty)),
                    defaultValueProperty
                );
                y += EditorGUI.GetPropertyHeight(defaultValueProperty);
                y += EditorGUIUtility.standardVerticalSpacing;
                
                for(var stateIndex = 0; stateIndex < state.States.Count; ++stateIndex)
                {
                    var stateHeight = ShowState(
                        new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
                        valuesProperty,
                        stateIndex
                    );

                    y += stateHeight;
                    y += EditorGUIUtility.standardVerticalSpacing;
                }
                
                foreach (var childProperty in otherProperties)
                {
                    var sizeY = EditorGUI.GetPropertyHeight(childProperty);
                    EditorGUI.PropertyField(
                        new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
                        childProperty
                    );
                    
                    y += sizeY + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            
            EditorGUI.EndProperty();
        }

        private void ShowTopLine(Rect position, SerializedProperty property, GUIContent defaultLabel)
        {
            var value = property.managedReferenceValue;
            var label = value != null
                ? new GUIContent(VisualStateEditorHelper.GetParameterTypeName(value.GetType()))
                : defaultLabel;

            property.isExpanded = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width / 2, position.height),
                property.isExpanded,
                label
            );

            EditorGUI.PropertyField(
                new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height),
                targetProperty,
                GUIContent.none
            );
        }

        private void ShowDefaultValue(Rect rect, SerializedProperty property)
        {
            EditorGUI.LabelField(
                new Rect(rect.x + CheckboxSize, rect.y, (rect.width - CheckboxSize) / 2,
                    EditorGUIUtility.singleLineHeight),
                "Default value"
            );
            
            EditorGUI.PropertyField(
                new Rect(
                    rect.x - CheckboxSize + (rect.width - CheckboxSize) / 2,
                    rect.y,
                    rect.width / 2,
                    EditorGUI.GetPropertyHeight(valuesProperty)
                ),
                property,
                GUIContent.none
            );
        }
        
        private void RetrieveProperties(SerializedProperty property)
        {
            state = property.serializedObject.targetObject as VisualState;
            targetProperty = property.FindPropertyRelative(TargetPropertyName);
            defaultValueProperty = property.FindPropertyRelative(DefaultValuePropertyName);
            valuesProperty = property.FindPropertyRelative(ValuesPropertyName);
            otherProperties.Clear();

            var copy = property.Copy();
            copy.NextVisible(true);

            do
            {
                if (!copy.propertyPath.StartsWith(property.propertyPath)) break;
                if(copy.name == TargetPropertyName || copy.name == DefaultValuePropertyName || copy.name == ValuesPropertyName) continue;
                otherProperties.Add(copy.Copy());
            } while (copy.NextVisible(false));
        }

        private float ShowState(Rect rect, SerializedProperty valuesProperty, int stateIndex)
        {
            SerializedProperty valueProperty = null;
            int valuePropertyIndex = 0;
            var height = EditorGUIUtility.singleLineHeight;
            
            for (var index = 0; index < valuesProperty.arraySize; ++index)
            {
                var property = valuesProperty.GetArrayElementAtIndex(index);
                var stateIdProperty = property.FindPropertyRelative("stateId");
                var thisStateIndex = stateIdProperty.intValue;
                if (thisStateIndex == stateIndex)
                {
                    valueProperty = property.FindPropertyRelative("value");
                    valuePropertyIndex = index;
                }
            }

            var hasProperty = valueProperty != null;
            var shouldHaveProperty = EditorGUI.Toggle(
                new Rect(rect.x, rect.y, CheckboxSize, EditorGUIUtility.singleLineHeight),
                valueProperty != null
            );

            EditorGUI.LabelField(
                new Rect(rect.x + CheckboxSize, rect.y, (rect.width - CheckboxSize)/2,  EditorGUIUtility.singleLineHeight),
                state.States[stateIndex]
            );
            
            if (valueProperty != null)
            {
                EditorGUI.PropertyField(
                    new Rect(
                        rect.x - CheckboxSize + (rect.width - CheckboxSize) / 2,
                        rect.y,
                        rect.width / 2,
                        EditorGUI.GetPropertyHeight(valueProperty)
                    ),
                    valueProperty,
                    GUIContent.none
                );
                height = Mathf.Max(height, EditorGUI.GetPropertyHeight(valueProperty));
            }

            if (hasProperty && !shouldHaveProperty)
            {
                valuesProperty.DeleteArrayElementAtIndex(valuePropertyIndex);
            }

            if (!hasProperty && shouldHaveProperty)
            {
                var newIndex = valuesProperty.arraySize;
                valuesProperty.InsertArrayElementAtIndex(newIndex);
                valuesProperty.GetArrayElementAtIndex(newIndex).FindPropertyRelative("stateId").intValue = stateIndex;
            }

            return height;
        }
        
    }
}