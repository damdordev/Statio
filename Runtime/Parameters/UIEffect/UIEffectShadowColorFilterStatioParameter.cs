#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.shadowColorFilter through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/ShadowColorFilter")]
    public class UIEffectShadowColorFilterStatioParameter : StatioParameter<UIEffect, ColorFilter>
    {
        protected override ColorFilter GetValue(UIEffect target) => target.shadowColorFilter;
        protected override void SetValue(UIEffect target, ColorFilter value) => target.shadowColorFilter = value;
    }
}
#endif