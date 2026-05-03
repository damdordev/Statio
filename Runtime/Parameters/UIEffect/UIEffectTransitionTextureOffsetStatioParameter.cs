#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/TransitionTextureOffset")]
    public class UIEffectTransitionTextureOffsetStatioParameter : StatioParameter<UIEffect, Vector2>
    {
        protected override Vector2 GetValue(UIEffect target) => target.transitionTextureOffset;
        protected override void SetValue(UIEffect target, Vector2 value) => target.transitionTextureOffset = value;
    }
}
#endif