#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.colorFilter through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/ColorFilter")]
    public class UIEffectColorFilterStatioParameter : StatioParameter<UIEffect, ColorFilter>
    {
        protected override ColorFilter GetValue(UIEffect target) => target.colorFilter;
        protected override void SetValue(UIEffect target, ColorFilter value) => target.colorFilter = value;
    }
}
#endif
