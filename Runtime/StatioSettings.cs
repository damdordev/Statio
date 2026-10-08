using System;
using System.Collections.Generic;
using System.Reflection;
using Damdor.Vario;
using UnityEngine;

namespace Damdor.Statio
{
    public static class StatioSettings
    {
        public static IReadOnlyList<Type> SupportedParameterTypes
        {
            get
            {
                EnsureSupportedParametersListGenerated();
                return supportedParameterTypes;
            }
        }

        public static bool LogErrorsToConsole
        {
            get => logErrorsToConsole;
            set => logErrorsToConsole = value;
        }

        private static List<Type> supportedParameterTypes = null;
        private static Dictionary<Type, string> parameterTypeToName = null;
        private static bool logErrorsToConsole;
        private static StatioErrorHandler errorHandler;

        public static void RegisterErrorHandler(StatioErrorHandler handler)
        {
            errorHandler = handler;
        }
        
        private static void EnsureSupportedParametersListGenerated()
        {
            if (supportedParameterTypes != null) return;

            supportedParameterTypes = new List<Type>();
            parameterTypeToName = new Dictionary<Type, string>();
            var baseType = typeof(StatioParameter);
            
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if(type.IsInterface || type.IsAbstract || !type.IsSerializable || !baseType.IsAssignableFrom(type)) continue;
                    var attr = type.GetCustomAttribute<StatioParameterAttribute>();
                    if(attr == null) continue;
                    
                    supportedParameterTypes.Add(type);
                    parameterTypeToName.Add(type, attr.Name);
                }
            }
        }
        
        public static string GetParameterTypeName(Type type)
        {
            EnsureSupportedParametersListGenerated();
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