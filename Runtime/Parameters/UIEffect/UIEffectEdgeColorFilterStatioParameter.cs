#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.edgeColorFilter through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/EdgeColorFilter")]
    public class UIEffectEdgeColorFilterStatioParameter : StatioParameter<UIEffect, ColorFilter>
    {
        protected override ColorFilter GetValue(UIEffect target) => target.edgeColorFilter;
        protected override void SetValue(UIEffect target, ColorFilter value) => target.edgeColorFilter = value;
    }
}
#endif