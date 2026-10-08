#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.transitionTextureScale through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/TransitionTextureScale")]
    public class UIEffectTransitionTextureScaleStatioParameter : StatioParameter<UIEffect, Vector2>
    {
        protected override Vector2 GetValue(UIEffect target) => target.transitionTextureScale;
        protected override void SetValue(UIEffect target, Vector2 value) => target.transitionTextureScale = value;
    }
}
#endif