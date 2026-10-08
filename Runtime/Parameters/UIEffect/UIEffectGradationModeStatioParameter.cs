#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.gradationMode through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/GradationMode")]
    public class UIEffectGradationModeStatioParameter : StatioParameter<UIEffect, GradationMode>
    {
        protected override GradationMode GetValue(UIEffect target) => target.gradationMode;
        protected override void SetValue(UIEffect target, GradationMode value) => target.gradationMode = value;
    }
}
#endif