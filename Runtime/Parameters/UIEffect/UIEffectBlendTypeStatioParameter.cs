#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/BlendType")]
    public class UIEffectBlendTypeStatioParameter : StatioParameter<UIEffect, BlendType>
    {
        protected override BlendType GetValue(UIEffect target) => target.blendType;
        protected override void SetValue(UIEffect target, BlendType value) => target.blendType = value;
    }
}
#endif
