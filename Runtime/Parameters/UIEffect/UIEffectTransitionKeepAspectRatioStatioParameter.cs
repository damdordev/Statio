#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.transitionKeepAspectRatio through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/TransitionKeepAspectRatio")]
    public class UIEffectTransitionKeepAspectRatioStatioParameter : StatioParameter<UIEffect, bool>
    {
        protected override bool GetValue(UIEffect target) => target.transitionKeepAspectRatio;
        protected override void SetValue(UIEffect target, bool value) => target.transitionKeepAspectRatio = value;
    }
}
#endif