using UnityEngine;

namespace Damdor.Statio
{
    [StatioParameterName("RectTransform/Pivot")]
    public class RectTransformPivotStatioParameter : StatioParameter<RectTransform, Vector2>
    {
        protected override Vector2 GetValue(RectTransform target) => target.pivot;

        protected override void SetValue(RectTransform target, Vector2 value) => target.pivot = value;
    }
}