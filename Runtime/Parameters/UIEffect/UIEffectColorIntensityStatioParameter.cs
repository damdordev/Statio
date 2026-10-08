#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.colorIntensity through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/ColorIntensity")]
    public class UIEffectColorIntensityStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.colorIntensity;
        protected override void SetValue(UIEffect target, float value) => target.colorIntensity = value;
    }
}
#endif