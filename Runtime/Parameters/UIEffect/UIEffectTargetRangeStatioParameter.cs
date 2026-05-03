#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/TargetRange")]
    public class UIEffectTargetRangeStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.targetRange;
        protected override void SetValue(UIEffect target, float value) => target.targetRange = value;
    }
}
#endif