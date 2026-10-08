#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.gradationRotation through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/GradationRotation")]
    public class UIEffectGradationRotationStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.gradationRotation;
        protected override void SetValue(UIEffect target, float value) => target.gradationRotation = value;
    }
}
#endif