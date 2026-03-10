using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("RectTransform/AnchoredPosition")]
    public class RectTransformAnchoredPositionVisualStateParameter : VisualStateParameter<RectTransform, Vector2>
    {
        protected override Vector2 GetValue(RectTransform target) => target.anchoredPosition;
        protected override void SetValue(RectTransform target, Vector2 value) => target.anchoredPosition = value;
    }
}