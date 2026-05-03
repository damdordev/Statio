#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/TransitionRate")]
    public class UIEffectTransitionRateStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.transitionRate;
        protected override void SetValue(UIEffect target, float value) => target.transitionRate = value;
    }
}
#endif