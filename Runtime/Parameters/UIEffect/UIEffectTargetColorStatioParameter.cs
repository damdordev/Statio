#if DAMDOR_STATIO_UIEFFECT
using System;
using UnityEngine;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/TargetColor")]
    public class UIEffectTargetColorStatioParameter : StatioParameter<UIEffect, Color>
    {
        protected override Color GetValue(UIEffect target) => target.targetColor;
        protected override void SetValue(UIEffect target, Color value) => target.targetColor = value;
    }
}
#endif