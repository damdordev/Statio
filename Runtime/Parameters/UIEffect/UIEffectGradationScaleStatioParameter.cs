#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.gradationScale through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/GradationScale")]
    public class UIEffectGradationScaleStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.gradationScale;
        protected override void SetValue(UIEffect target, float value) => target.gradationScale = value;
    }
}
#endif