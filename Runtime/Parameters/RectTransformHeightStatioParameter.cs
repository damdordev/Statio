using UnityEngine;

namespace Damdor.Statio
{
    [StatioParameterName("RectTransform/Height")]
    public class RectTransformHeightStatioParameter : StatioParameter<RectTransform, float>
    {
        protected override float GetValue(RectTransform target) => target.rect.height;
        protected override void SetValue(RectTransform target, float value) => target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
    }
}