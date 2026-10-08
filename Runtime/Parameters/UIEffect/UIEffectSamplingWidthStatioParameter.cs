#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.samplingWidth through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/SamplingWidth")]
    public class UIEffectSamplingWidthStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.samplingWidth;
        protected override void SetValue(UIEffect target, float value) => target.samplingWidth = value;
    }
}
#endif