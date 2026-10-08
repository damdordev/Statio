#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.toneFilter through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/ToneFilter")]
    public class UIEffectToneFilterStatioParameter : StatioParameter<UIEffect, ToneFilter>
    {
        protected override ToneFilter GetValue(UIEffect target) => target.toneFilter;
        protected override void SetValue(UIEffect target, ToneFilter value) => target.toneFilter = value;
    }
}
#endif
