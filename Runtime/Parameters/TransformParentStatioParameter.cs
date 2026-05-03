using System;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("Transform/Parent")]
    public class TransformParentStatioParameter : StatioParameter<Transform, Transform>
    {
        protected override Transform GetValue(Transform target) => target.parent;

        protected override void SetValue(Transform target, Transform value)
        {
            target.SetParent(value, false);
            target.SetAsLastSibling();
        }

        protected override Transform Lerp(Transform a, Transform b, float t) => t >= 0.5f ? a : b;
    }
}