#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.targetMode through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/TargetMode")]
    public class UIEffectTargetModeStatioParameter : StatioParameter<UIEffect, TargetMode>
    {
        protected override TargetMode GetValue(UIEffect target) => target.targetMode;
        protected override void SetValue(UIEffect target, TargetMode value) => target.targetMode = value;
    }
}
#endif
