using System;
using System.Collections.Generic;

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

        private static bool init;
        private static readonly List<Type> supportedParameterTypes = new();
        
        public static void ResetToInitialSettings()
        {
            ResetSupportedParameterTypes();
            init = true;
        }

        public static void ClearSupportedParameterTypes()
        {
            supportedParameterTypes.Clear();
        }

        private static void ResetSupportedParameterTypes()
        {
            supportedParameterTypes.Clear();
            
            supportedParameterTypes.Add(typeof(PositionVisualStateParameter));
            supportedParameterTypes.Add(typeof(ScaleVisualStateParameter));
            supportedParameterTypes.Add(typeof(ImageColorVisualStateParameter));
        }
        
        private static void EnsureInit()
        {
            if (init) return;
            ResetToInitialSettings();
        }
        
    }
}