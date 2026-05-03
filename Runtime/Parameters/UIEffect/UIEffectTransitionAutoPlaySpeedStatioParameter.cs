#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/TransitionAutoPlaySpeed")]
    public class UIEffectTransitionAutoPlaySpeedStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.transitionAutoPlaySpeed;
        protected override void SetValue(UIEffect target, float value) => target.transitionAutoPlaySpeed = value;
    }
}
#endif