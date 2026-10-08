#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.shadowBlurIntensity through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/ShadowBlurIntensity")]
    public class UIEffectShadowBlurIntensityStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.shadowBlurIntensity;
        protected override void SetValue(UIEffect target, float value) => target.shadowBlurIntensity = value;
    }
}
#endif