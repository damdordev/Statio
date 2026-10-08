#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.gradationOffset through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/GradationOffset")]
    public class UIEffectGradationOffsetStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.gradationOffset;
        protected override void SetValue(UIEffect target, float value) => target.gradationOffset = value;
    }
}
#endif