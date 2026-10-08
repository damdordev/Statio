#if DAMDOR_STATIO_UIEFFECT
using System;
using UnityEngine;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.gradationColor1 through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/GradationColor1")]
    public class UIEffectGradationColor1StatioParameter : StatioParameter<UIEffect, Color>
    {
        protected override Color GetValue(UIEffect target) => target.gradationColor1;
        protected override void SetValue(UIEffect target, Color value) => target.gradationColor1 = value;
    }
}
#endif