using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("RectTransform/AnchoredPosition")]
    public class RectTransformAnchoredPositionVisualStateParameter : Vector2VisualStateParameter<RectTransform>
    {
        protected override Vector2 GetValue(RectTransform target) => target.anchoredPosition;

        protected override void SetValue(RectTransform target, Vector2 value)
            => target.anchoredPosition = value;
    }
}