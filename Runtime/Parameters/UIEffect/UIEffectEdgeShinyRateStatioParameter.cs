#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/EdgeShinyRate")]
    public class UIEffectEdgeShinyRateStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.edgeShinyRate;
        protected override void SetValue(UIEffect target, float value) => target.edgeShinyRate = value;
    }
}
#endif