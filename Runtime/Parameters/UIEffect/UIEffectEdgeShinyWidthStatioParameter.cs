#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.edgeShinyWidth through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/EdgeShinyWidth")]
    public class UIEffectEdgeShinyWidthStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.edgeShinyWidth;
        protected override void SetValue(UIEffect target, float value) => target.edgeShinyWidth = value;
    }
}
#endif