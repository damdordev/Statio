#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.transitionColorGlow through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/TransitionColorGlow")]
    public class UIEffectTransitionColorGlowStatioParameter : StatioParameter<UIEffect, bool>
    {
        protected override bool GetValue(UIEffect target) => target.transitionColorGlow;
        protected override void SetValue(UIEffect target, bool value) => target.transitionColorGlow = value;
    }
}
#endif