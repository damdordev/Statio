#if DAMDOR_STATIO_UIEFFECT
using System;
using UnityEngine;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/TransitionColor")]
    public class UIEffectTransitionColorStatioParameter : StatioParameter<UIEffect, Color>
    {
        protected override Color GetValue(UIEffect target) => target.transitionColor;
        protected override void SetValue(UIEffect target, Color value) => target.transitionColor = value;
    }
}
#endif