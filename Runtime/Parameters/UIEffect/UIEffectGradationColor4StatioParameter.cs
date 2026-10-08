#if DAMDOR_STATIO_UIEFFECT
using System;
using UnityEngine;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.gradationColor4 through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/GradationColor4")]
    public class UIEffectGradationColor4StatioParameter : StatioParameter<UIEffect, Color>
    {
        protected override Color GetValue(UIEffect target) => target.gradationColor4;
        protected override void SetValue(UIEffect target, Color value) => target.gradationColor4 = value;
    }
}
#endif