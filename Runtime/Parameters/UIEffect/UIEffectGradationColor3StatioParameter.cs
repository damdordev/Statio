#if DAMDOR_STATIO_UIEFFECT
using System;
using UnityEngine;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/GradationColor3")]
    public class UIEffectGradationColor3StatioParameter : StatioParameter<UIEffect, Color>
    {
        protected override Color GetValue(UIEffect target) => target.gradationColor3;
        protected override void SetValue(UIEffect target, Color value) => target.gradationColor3 = value;
    }
}
#endif