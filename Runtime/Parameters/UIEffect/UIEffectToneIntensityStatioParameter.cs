#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.toneIntensity through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/ToneIntensity")]
    public class UIEffectToneIntensityStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.toneIntensity;
        protected override void SetValue(UIEffect target, float value) => target.toneIntensity = value;
    }
}
#endif