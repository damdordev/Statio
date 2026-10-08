#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffectInternal;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.transitionRange through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/TransitionRange")]
    public class UIEffectTransitionRangeStatioParameter : StatioParameter<UIEffect, MinMax01>
    {
        protected override MinMax01 GetValue(UIEffect target) => target.transitionRange;
        protected override void SetValue(UIEffect target, MinMax01 value) => target.transitionRange = value;
    }
}
#endif