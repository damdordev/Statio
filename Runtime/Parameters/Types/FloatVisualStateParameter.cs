using UnityEngine;

namespace Damdor.VisualStates
{
    public abstract class FloatVisualStateParameter<TComponent> : VisualStateParameter<TComponent, float>
        where TComponent : Object
    {
        protected override float Lerp(float a, float b, float t)
            => Mathf.LerpUnclamped(a, b, t);
    }
}