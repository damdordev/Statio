#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/TransitionSoftness")]
    public class UIEffectTransitionSoftnessStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.transitionSoftness;
        protected override void SetValue(UIEffect target, float value) => target.transitionSoftness = value;
    }
}
#endif