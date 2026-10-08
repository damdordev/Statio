#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.edgeShinyAutoPlaySpeed through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/EdgeShinyAutoPlaySpeed")]
    public class UIEffectEdgeShinyAutoPlaySpeedStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.edgeShinyAutoPlaySpeed;
        protected override void SetValue(UIEffect target, float value) => target.edgeShinyAutoPlaySpeed = value;
    }
}
#endif