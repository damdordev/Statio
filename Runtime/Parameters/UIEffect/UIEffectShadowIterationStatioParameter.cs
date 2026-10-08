#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.shadowIteration through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/ShadowIteration")]
    public class UIEffectShadowIterationStatioParameter : StatioParameter<UIEffect, int>
    {
        protected override int GetValue(UIEffect target) => target.shadowIteration;
        protected override void SetValue(UIEffect target, int value) => target.shadowIteration = value;
    }
}
#endif