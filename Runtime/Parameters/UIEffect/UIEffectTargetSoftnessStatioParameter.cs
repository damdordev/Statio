#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.targetSoftness through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/TargetSoftness")]
    public class UIEffectTargetSoftnessStatioParameter : StatioParameter<UIEffect, float>
    {
        protected override float GetValue(UIEffect target) => target.targetSoftness;
        protected override void SetValue(UIEffect target, float value) => target.targetSoftness = value;
    }
}
#endif