using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("RectTransform/Pivot")]
    public class RectTransformPivotVisualStateParameter : VisualStateParameter<RectTransform, Vector2>
    {
        protected override Vector2 GetValue(RectTransform target) => target.pivot;

        protected override void SetValue(RectTransform target, Vector2 value) => target.pivot = value;
    }
}