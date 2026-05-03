#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/SamplingScale")]
    public class UIEffectSamplingScaleStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.samplingScale;
        protected override void SetValue(UIEffect target, float value) => target.samplingScale = value;
    }
}
#endif