#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/ShadowGlow")]
    public class UIEffectShadowGlowStatioParameter : StatioParameter<UIEffect, bool>
    {
        protected override bool GetValue(UIEffect target) => target.shadowGlow;
        protected override void SetValue(UIEffect target, bool value) => target.shadowGlow = value;
    }
}
#endif