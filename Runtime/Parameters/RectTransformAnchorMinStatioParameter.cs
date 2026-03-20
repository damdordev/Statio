using System;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    public class RectTransformAnchorMinStatioParameter : StatioParameter<RectTransform, Vector2>
    {
        protected override Vector2 GetValue(RectTransform target) => target.anchorMin;
        protected override void SetValue(RectTransform target, Vector2 value) => target.anchorMin = value;
    }
}