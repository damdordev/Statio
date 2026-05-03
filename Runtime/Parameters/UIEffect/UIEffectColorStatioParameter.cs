#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/Color")]
    public class UIEffectColorStatioParameter : StatioParameter<UIEffect, Color>
    {
        protected override Color GetValue(UIEffect target) => target.color;
        protected override void SetValue(UIEffect target, Color value) => target.color = value;
    }
}
#endif