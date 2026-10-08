#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.samplingIntensity through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/SamplingIntensity")]
    public class UIEffectSamplingIntensityStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.samplingIntensity;
        protected override void SetValue(UIEffect target, float value) => target.samplingIntensity = value;
    }
}
#endif