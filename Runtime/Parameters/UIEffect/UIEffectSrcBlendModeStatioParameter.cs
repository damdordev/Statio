#if DAMDOR_STATIO_UIEFFECT
using System;
using UnityEngine.Rendering;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.srcBlendMode through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/SrcBlendMode")]
    public class UIEffectSrcBlendModeStatioParameter : StatioParameter<UIEffect, BlendMode>
    {
        protected override BlendMode GetValue(UIEffect target) => target.srcBlendMode;
        protected override void SetValue(UIEffect target, BlendMode value) => target.srcBlendMode = value;
    }
}
#endif