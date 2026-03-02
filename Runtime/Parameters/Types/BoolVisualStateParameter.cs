using UnityEngine;

namespace Damdor.VisualStates
{
    public abstract class BoolVisualStateParameter<TComponent> : VisualStateParameter<TComponent, bool>
        where TComponent : Object
    {
    }
}