#if DAMDOR_STATIO_UIEFFECT
using System;
using UnityEngine.Rendering;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/DstBlendMode")]
    public class UIEffectDstBlendModeStatioParameter : StatioParameter<UIEffect, BlendMode>
    {
        protected override BlendMode GetValue(UIEffect target) => target.dstBlendMode;
        protected override void SetValue(UIEffect target, BlendMode value) => target.dstBlendMode = value;
    }
}
#endif