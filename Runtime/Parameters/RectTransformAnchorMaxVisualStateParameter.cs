using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("RectTransform/AnchorMax")]
    public class RectTransformAnchorMaxVisualStateParameter : VisualStateParameter<RectTransform, Vector2>
    {
        protected override Vector2 GetValue(RectTransform target) => target.anchorMax;
        protected override void SetValue(RectTransform target, Vector2 value) => target.anchorMax = value;
    }
}