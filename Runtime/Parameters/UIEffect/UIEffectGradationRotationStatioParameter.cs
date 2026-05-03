#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/GradationRotation")]
    public class UIEffectGradationRotationStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.gradationRotation;
        protected override void SetValue(UIEffect target, float value) => target.gradationRotation = value;
    }
}
#endif