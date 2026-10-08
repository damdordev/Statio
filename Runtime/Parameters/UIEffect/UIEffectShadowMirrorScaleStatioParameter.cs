#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.shadowMirrorScale through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/ShadowMirrorScale")]
    public class UIEffectShadowMirrorScaleStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.shadowMirrorScale;
        protected override void SetValue(UIEffect target, float value) => target.shadowMirrorScale = value;
    }
}
#endif