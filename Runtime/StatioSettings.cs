using System;
using System.Collections.Generic;
using Damdor.Foundation;
using UnityEngine;

namespace Damdor.Statio
{
    public static class StatioSettings
    {
        public static IReadOnlyList<Type> SupportedParameterTypes
        {
            get
            {
                EnsureInit();
                return supportedParameterTypes;
            }
        }

        public static bool LogErrorsToConsole
        {
            get => logErrorsToConsole;
            set => logErrorsToConsole = value;
        }

        private static bool init;
        private static readonly List<Type> supportedParameterTypes = new();
        private static bool logErrorsToConsole;
        private static StatioErrorHandler errorHandler;
        
        public static void ResetToInitialSettings()
        {
            ResetSupportedParameterTypes();
            init = true;
            errorHandler = null;
        }

        public static void RegisterErrorHandler(StatioErrorHandler handler)
        {
            EnsureInit();
            errorHandler = handler;
        }

        public static void RegisterParameterType(Type type)
        {
            EnsureInit();
            supportedParameterTypes.Add(type);
        }

        private static void ClearSupportedParameterTypes()
        {
            supportedParameterTypes.Clear();
        }

        private static void ResetSupportedParameterTypes()
        {
            supportedParameterTypes.Clear();
            
            supportedParameterTypes.Add(typeof(PositionStatioParameter));
            supportedParameterTypes.Add(typeof(ScaleStatioParameter));
            supportedParameterTypes.Add(typeof(GraphicColorStatioParameter));
            supportedParameterTypes.Add(typeof(GameObjectActivityStatioParameter));
            supportedParameterTypes.Add(typeof(TransformParentStatioParameter));
            supportedParameterTypes.Add(typeof(StatioStatioParameter));
            supportedParameterTypes.Add(typeof(EulerAnglesStatioParameter));
            supportedParameterTypes.Add(typeof(RectTransformWidthStatioParameter));
            supportedParameterTypes.Add(typeof(RectTransformHeightStatioParameter));
            supportedParameterTypes.Add(typeof(RectTransformPivotStatioParameter));
            supportedParameterTypes.Add(typeof(RectTransformAnchoredPositionStatioParameter));
            supportedParameterTypes.Add(typeof(RectTransformAnchorMinStatioParameter));
            supportedParameterTypes.Add(typeof(RectTransformAnchorMaxStatioParameter));
            supportedParameterTypes.Add(typeof(CanvasGroupAlphaStatioParameter));
        }
        
        private static void EnsureInit()
        {
            if (init) return;
            ResetToInitialSettings();
        }

        internal static void NotifyError(VisualState visualState, string error)
        {
            EnsureInit();
            if (logErrorsToConsole)
            {
                var path = visualState == null ? null : visualState.transform.GetFullPath();
                Debug.LogError($"Visual state error [{path}]: {error}");
            }

            if (errorHandler != null) errorHandler(visualState, error);
        }
        
    }
}