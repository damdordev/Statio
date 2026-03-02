using UnityEngine;

namespace Damdor.VisualStates
{
    public abstract class Vector2VisualStateParameter<TComponent> : VisualStateParameter<TComponent, Vector2>
        where TComponent : Object
    {
        protected override Vector2 Lerp(Vector2 a, Vector2 b, float t)
            => Vector2.LerpUnclamped(a, b, t);
    }
}