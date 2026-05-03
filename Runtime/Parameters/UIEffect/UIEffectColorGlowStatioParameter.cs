#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/ColorGlow")]
    public class UIEffectColorGlowStatioParameter : StatioParameter<UIEffect, bool>
    {
        protected override bool GetValue(UIEffect target) => target.colorGlow;
        protected override void SetValue(UIEffect target, bool value) => target.colorGlow = value;
    }
}
#endif