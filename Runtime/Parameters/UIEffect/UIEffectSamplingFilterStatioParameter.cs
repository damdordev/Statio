#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/SamplingFilter")]
    public class UIEffectSamplingFilterStatioParameter : StatioParameter<UIEffect, SamplingFilter>
    {
        protected override SamplingFilter GetValue(UIEffect target) => target.samplingFilter;
        protected override void SetValue(UIEffect target, SamplingFilter value) => target.samplingFilter = value;
    }
}
#endif
