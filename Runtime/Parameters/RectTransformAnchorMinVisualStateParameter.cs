using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("RectTransform/AnchorMin")]
    public class RectTransformAnchorMinVisualStateParameter : VisualStateParameter<RectTransform, Vector2>
    {
        protected override Vector2 GetValue(RectTransform target) => target.anchorMin;
        protected override void SetValue(RectTransform target, Vector2 value) => target.anchorMin = value;
    }
}