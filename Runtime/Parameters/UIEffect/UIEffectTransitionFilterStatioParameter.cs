#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.transitionFilter through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/TransitionFilter")]
    public class UIEffectTransitionFilterStatioParameter : StatioParameter<UIEffect, TransitionFilter>
    {
        protected override TransitionFilter GetValue(UIEffect target) => target.transitionFilter;
        protected override void SetValue(UIEffect target, TransitionFilter value) => target.transitionFilter = value;
    }
}
#endif
