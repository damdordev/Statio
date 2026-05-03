using System;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("RectTransform/Width")]
    public class RectTransformWidthStatioParameter : StatioParameter<RectTransform, float>
    {
        protected override float GetValue(RectTransform target) => target.rect.width;
        protected override void SetValue(RectTransform target, float value) => target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
    }
}