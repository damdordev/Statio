using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("RectTransform/Width")]
    public class RectTransformWidthVisualStateParameter : VisualStateParameter<RectTransform, float>
    {
        protected override float GetValue(RectTransform target) => target.rect.width;
        protected override void SetValue(RectTransform target, float value) => target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
    }
}