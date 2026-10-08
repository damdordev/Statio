using System;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls RectTransform.sizeDelta through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("RectTransform/Size Delta")]
    public class RectTransformSizeDeltaStatioParameter : StatioParameter<RectTransform, Vector2>
    {
        protected override Vector2 GetValue(RectTransform target) => target.sizeDelta;
        protected override void SetValue(RectTransform target, Vector2 value) => target.sizeDelta = value;
    }
}
