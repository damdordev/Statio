#if DAMDOR_STATIO_UIEFFECT
using System;
using UnityEngine;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/ShadowColor")]
    public class UIEffectShadowColorStatioParameter : StatioParameter<UIEffect, Color>
    {
        protected override Color GetValue(UIEffect target) => target.shadowColor;
        protected override void SetValue(UIEffect target, Color value) => target.shadowColor = value;
    }
}
#endif