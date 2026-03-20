using System;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    public class RectTransformAnchoredPositionStatioParameter : StatioParameter<RectTransform, Vector2>
    {
        protected override Vector2 GetValue(RectTransform target) => target.anchoredPosition;
        protected override void SetValue(RectTransform target, Vector2 value) => target.anchoredPosition = value;
    }
}