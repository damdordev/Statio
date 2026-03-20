using System.Collections.Generic;
using Damdor.Foundation;
using UnityEditor;
using UnityEngine;

namespace Damdor.Statio
{
    internal class StatioSettingsLoader : AssetPostprocessor
    {
        public static void Load()
        {
            var settingsAssets = Resources.LoadAll<TextAsset>("statio_settings");
            foreach (var t in settingsAssets)
            {
                var json = t.text;
                var settingsData = (Dictionary<string, object>)Json.Deserialize(json);
                Resources.UnloadAsset(t);
                
                if (settingsData != null && settingsData.TryGetValue("parameters", out var types))
                {
                    ProcessParametersFromSettingsData((Dictionary<string, object>)types);
                }
            }
        }
        
        private static void ProcessParametersFromSettingsData(Dictionary<string, object> parameters)
        {
            foreach (var pair in parameters)
            {
                var type = ReflectionHelper.FindType((string)pair.Value);
                if (type != null) StatioSettings.RegisterParameterType(type, pair.Key);
            }
        }
        
        void OnPreprocessAsset()
        {
            if (assetImporter.assetPath.EndsWith("statio_settings.json"))
            {
                StatioSettings.Reset();
            }
        }
        
    }
}