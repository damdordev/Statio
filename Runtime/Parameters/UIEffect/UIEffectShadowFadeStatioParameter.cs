#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/ShadowFade")]
    public class UIEffectShadowFadeStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.shadowFade;
        protected override void SetValue(UIEffect target, float value) => target.shadowFade = value;
    }
}
#endif