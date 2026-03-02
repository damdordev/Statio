using System;
using System.Collections.Generic;
using Damdor.Foundation;
using UnityEngine;

namespace Damdor.VisualStates
{
    public static class VisualStateSettings
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
        private static VisualStateErrorHandler errorHandler;
        
        public static void ResetToInitialSettings()
        {
            ResetSupportedParameterTypes();
            init = true;
            errorHandler = null;
        }

        public static void RegisterErrorHandler(VisualStateErrorHandler handler)
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
            
            supportedParameterTypes.Add(typeof(PositionVisualStateParameter));
            supportedParameterTypes.Add(typeof(ScaleVisualStateParameter));
            supportedParameterTypes.Add(typeof(GraphicColorVisualStateParameter));
            supportedParameterTypes.Add(typeof(GameObjectActivityVisualStateParameter));
            supportedParameterTypes.Add(typeof(TransformParentVisualStateParameter));
            supportedParameterTypes.Add(typeof(VisualStateVisualStateParameter));
            supportedParameterTypes.Add(typeof(EulerAnglesVisualStateParameter));
            supportedParameterTypes.Add(typeof(RectTransformWidthVisualStateParameter));
            supportedParameterTypes.Add(typeof(RectTransformHeightVisualStateParameter));
            supportedParameterTypes.Add(typeof(RectTransformPivotVisualStateParameter));
            supportedParameterTypes.Add(typeof(RectTransformAnchoredPositionVisualStateParameter));
            supportedParameterTypes.Add(typeof(RectTransformAnchorMinVisualStateParameter));
            supportedParameterTypes.Add(typeof(RectTransformAnchorMaxVisualStateParameter));
            supportedParameterTypes.Add(typeof(CanvasGroupAlphaVisualStateParameter));
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