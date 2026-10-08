#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.transitionRotation through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/TransitionRotation")]
    public class UIEffectTransitionRotationStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.transitionRotation;
        protected override void SetValue(UIEffect target, float value) => target.transitionRotation = value;
    }
}
#endif