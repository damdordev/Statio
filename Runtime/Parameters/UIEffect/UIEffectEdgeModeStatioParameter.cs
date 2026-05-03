#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/EdgeMode")]
    public class UIEffectEdgeModeStatioParameter : StatioParameter<UIEffect, EdgeMode>
    {
        protected override EdgeMode GetValue(UIEffect target) => target.edgeMode;
        protected override void SetValue(UIEffect target, EdgeMode value) => target.edgeMode = value;
    }
}
#endif
