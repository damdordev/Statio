#if DAMDOR_STATIO_UIEFFECT
using System;
using UnityEngine;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("UIEffect/EdgeColor")]
    public class UIEffectEdgeColorStatioParameter : StatioParameter<UIEffect, Color>
    {
        protected override Color GetValue(UIEffect target) => target.edgeColor;
        protected override void SetValue(UIEffect target, Color value) => target.edgeColor = value;
    }
}
#endif