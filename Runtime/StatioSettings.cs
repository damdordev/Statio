using System;
using System.Collections.Generic;
using Damdor.Vario;
using UnityEngine;

namespace Damdor.Statio
{
    public static class StatioSettings
    {
        public static IReadOnlyList<Type> SupportedParameterTypes => supportedParameterTypes;

        public static bool LogErrorsToConsole
        {
            get => logErrorsToConsole;
            set => logErrorsToConsole = value;
        }

        private static readonly List<Type> supportedParameterTypes = new();
        private static readonly Dictionary<Type, string> parameterTypeToName = new();
        private static bool logErrorsToConsole;
        private static StatioErrorHandler errorHandler;

        public static void RegisterErrorHandler(StatioErrorHandler handler)
        {
            errorHandler = handler;
        }

        public static void RegisterParameterType(Type type, string name)
        {
            supportedParameterTypes.Add(type);
            parameterTypeToName.Add(type, name);
        }
        
        public static string GetParameterTypeName(Type type)
        {
            var name = parameterTypeToName.GetValueOrDefault(type, "");
            return name;
        }

        internal static void NotifyError(VisualState visualState, string error)
        {
            if (logErrorsToConsole)
            {
                var path = visualState == null ? null : visualState.transform.GetFullPath();
                Debug.LogError($"Visual state error [{path}]: {error}");
            }

            if (errorHandler != null) errorHandler(visualState, error);
        }
        
    }
}