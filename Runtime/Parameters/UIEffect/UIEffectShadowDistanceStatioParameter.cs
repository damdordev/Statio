#if DAMDOR_STATIO_UIEFFECT
using System;
using UnityEngine;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.shadowDistance through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/ShadowDistance")]
    public class UIEffectShadowDistanceStatioParameter : StatioParameter<UIEffect, Vector2>
    {
        protected override Vector2 GetValue(UIEffect target) => target.shadowDistance;
        protected override void SetValue(UIEffect target, Vector2 value) => target.shadowDistance = value;
    }
}
#endif