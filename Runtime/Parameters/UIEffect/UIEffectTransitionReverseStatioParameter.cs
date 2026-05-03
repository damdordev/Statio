#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/TransitionReverse")]
    public class UIEffectTransitionReverseStatioParameter : StatioParameter<UIEffect, bool>
    {
        protected override bool GetValue(UIEffect target) => target.transitionReverse;
        protected override void SetValue(UIEffect target, bool value) => target.transitionReverse = value;
    }
}
#endif