#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/TransitionTexture")]
    public class UIEffectTransitionTextureStatioParameter : StatioParameter<UIEffect, Texture>
    {
        protected override Texture GetValue(UIEffect target) => target.transitionTexture;
        protected override void SetValue(UIEffect target, Texture value) => target.transitionTexture = value;
    }
}
#endif