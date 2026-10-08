#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.shadowMode through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/ShadowMode")]
    public class UIEffectShadowModeStatioParameter : StatioParameter<UIEffect, ShadowMode>
    {
        protected override ShadowMode GetValue(UIEffect target) => target.shadowMode;
        protected override void SetValue(UIEffect target, ShadowMode value) => target.shadowMode = value;
    }
}
#endif
