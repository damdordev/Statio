using System;
using System.Collections.Generic;
using System.Linq;
using Damdor.Vario.Editor;
using UnityEditor;
using UnityEngine;

namespace Damdor.Statio.Editor
{
    internal static class StatioEditorHelper
    {
        private static readonly Dictionary<Type, string> variableTypeToName = new();
        
        public static string GetNewStateName(SerializedProperty statesProperty)
        {
            var name = 1;
            while (HasState(statesProperty, name.ToString())) ++name;
            return name.ToString();
        }

        public static bool HasState(SerializedProperty statesProperty, string state)
        {
            for (var i = 0; i < statesProperty.arraySize; i++)
            {
                if (statesProperty.GetArrayElementAtIndex(i).stringValue == state) return true;
            }

            return false;
        }
        
        public static void ShowStateChoice(VisualState visualState, int stateId, string emptyStateName, Action<int> onChange)
        {
            var text = stateId < 0 ? emptyStateName : visualState.States[stateId];
            if (GUILayout.Button(text, GUILayout.ExpandWidth(false), GUILayout.Width(100)))
            {
                var rect = GUILayoutUtility.GetLastRect();
                rect.x += 100f;
                ShowStateChoiceDropdown(rect, visualState, stateId, emptyStateName, onChange);
            }
        }
        
        public static void ShowStateChoice(Rect rect, VisualState visualState, int stateId, string emptyStateName, Action<int> onChange)
        {
            var text = stateId < 0 ? emptyStateName : visualState.States[stateId];
            if (GUI.Button(rect, text))
            {
                ShowStateChoiceDropdown(rect, visualState, stateId, emptyStateName, onChange);
            }
        }

        public static void ShowStateChoiceDropdown(
            Rect rect,
            VisualState visualState, 
            int selectedStateId,
            string emptyStateName,
            Action<int> onChange)
        {
            var allStates = new List<string> { emptyStateName };
            allStates.AddRange(visualState.States);

            new HierarchicalDropdown<int>(Enumerable.Range(0, allStates.Count), i => allStates[i], newStateId =>
            {
                --newStateId;
                if(selectedStateId != newStateId && onChange != null) onChange(newStateId);
            }).Show(rect);
        }
        
    }
}