using System;
using System.Collections.Generic;
using Damdor.Vario;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Maintains global statio settings
    /// </summary>
    public static class StatioSettings
    {
        /// <summary>
        /// Gets registered parameter types in registration order.
        /// </summary>
        public static IReadOnlyList<Type> SupportedParameterTypes => supportedParameterTypes;

        /// <summary>
        /// Gets or sets whether errors are logged to the Unity console. Enabled by default.
        /// </summary>
        public static bool LogErrorsToConsole { get; set; } = true;

        private static readonly List<Type> supportedParameterTypes = new();
        private static readonly Dictionary<Type, string> parameterTypeToName = new();
        private static StatioErrorHandler errorHandler;

        /// <summary>
        /// Replaces the global error callback, independently of console logging.
        /// </summary>
        /// <param name="handler">The callback to register, or null to clear it.</param>
        public static void RegisterErrorHandler(StatioErrorHandler handler)
        {
            errorHandler = handler;
        }

        /// <summary>
        /// Registers a parameter type and its Inspector menu name, replacing any existing registration.
        /// </summary>
        /// <param name="type">The parameter type to register.</param>
        /// <param name="name">The display name, with slashes separating menu groups.</param>
        public static void RegisterParameterType(Type type, string name)
        {
            if (supportedParameterTypes.Contains(type))
            {
                supportedParameterTypes.Remove(type);
                parameterTypeToName.Remove(type);
            }

            supportedParameterTypes.Add(type);
            parameterTypeToName.Add(type, name);
        }
        
        /// <summary>
        /// Gets the registered Inspector menu name for a parameter type.
        /// </summary>
        /// <param name="type">The parameter type to look up.</param>
        /// <returns>The registered name, or an empty string if the type is not registered.</returns>
        public static string GetParameterTypeName(Type type)
        {
            var name = parameterTypeToName.GetValueOrDefault(type, "");
            return name;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        internal static void NotifyError(VisualState visualState, string error)
        {
            if (LogErrorsToConsole)
            {
                var path = !visualState ? null : visualState.transform.GetFullPath();
                Debug.LogError($"Visual state error [{path}]: {error}");
            }

            errorHandler?.Invoke(visualState, error);
        }
        
    }
}