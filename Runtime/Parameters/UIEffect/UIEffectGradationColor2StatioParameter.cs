#if DAMDOR_STATIO_UIEFFECT
using System;
using UnityEngine;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.gradationColor2 through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/GradationColor2")]
    public class UIEffectGradationColor2StatioParameter : StatioParameter<UIEffect, Color>
    {
        protected override Color GetValue(UIEffect target) => target.gradationColor2;
        protected override void SetValue(UIEffect target, Color value) => target.gradationColor2 = value;
    }
}
#endif