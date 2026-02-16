using UnityEngine;

namespace Damdor.VisualStates
{
    public abstract class ColorVisualStateParameter<TComponent> : VisualStateParameter<TComponent, Color>
        where TComponent : Object
    {
        protected override Color Lerp(Color a, Color b, float t) => Color.LerpUnclamped(a, b, t);
    }
}