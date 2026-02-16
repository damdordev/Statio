using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Damdor.VisualStates.Editor
{
    internal static class VisualStateEditorHelper
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
        
        public static string GetParameterTypeName(Type type)
        {
            if(variableTypeToName.TryGetValue(type, out var result)) return result;

            var attr = type.GetCustomAttribute<VisualParameterTypeName>();
            var name = attr != null ? attr.Name : type.Name;
            variableTypeToName[type] = name;

            return name;
        }
        
    }
}