using UnityEngine;

namespace Damdor.VisualStates
{
    public abstract class Vector3VisualStateParameter<TComponent> : VisualStateParameter<TComponent, Vector3>
        where TComponent : Object
    {
        protected override Vector3 Lerp(Vector3 a, Vector3 b, float t)
            => Vector3.LerpUnclamped(a, b, t);
    }
}