#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.transitionPatternReverse through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/TransitionPatternReverse")]
    public class UIEffectTransitionPatternReverseStatioParameter : StatioParameter<UIEffect, bool>
    {
        protected override bool GetValue(UIEffect target) => target.transitionPatternReverse;
        protected override void SetValue(UIEffect target, bool value) => target.transitionPatternReverse = value;
    }
}
#endif