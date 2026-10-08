#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.transitionWidth through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/TransitionWidth")]
    public class UIEffectTransitionWidthStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.transitionWidth;
        protected override void SetValue(UIEffect target, float value) => target.transitionWidth = value;
    }
}
#endif