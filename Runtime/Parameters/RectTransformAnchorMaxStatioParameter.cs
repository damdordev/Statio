using System;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls RectTransform.anchorMax through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("RectTransform/AnchorMax")]
    public class RectTransformAnchorMaxStatioParameter : StatioParameter<RectTransform, Vector2>
    {
        protected override Vector2 GetValue(RectTransform target) => target.anchorMax;
        protected override void SetValue(RectTransform target, Vector2 value) => target.anchorMax = value;
    }
}