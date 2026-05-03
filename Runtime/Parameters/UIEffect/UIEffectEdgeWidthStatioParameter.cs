#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/EdgeWidth")]
    public class UIEffectEdgeWidthStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.edgeWidth;
        protected override void SetValue(UIEffect target, float value) => target.edgeWidth = value;
    }
}
#endif