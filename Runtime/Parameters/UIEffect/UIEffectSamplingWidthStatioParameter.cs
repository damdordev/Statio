#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/SamplingWidth")]
    public class UIEffectSamplingWidthStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.samplingWidth;
        protected override void SetValue(UIEffect target, float value) => target.samplingWidth = value;
    }
}
#endif