using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("RectTransform/Height")]
    public class RectTransformHeightVisualStateParameter : VisualStateParameter<RectTransform, float>
    {
        protected override float GetValue(RectTransform target) => target.rect.height;
        protected override void SetValue(RectTransform target, float value) => target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
    }
}