using UnityEngine;
using UnityEngine.UI;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("Graphic/Color")]
    public class GraphicColorVisualStateParameter : VisualStateParameter<Graphic, Color>
    {
        protected override Color GetValue(Graphic target) => target.color;
        protected override void SetValue(Graphic target, Color value) => target.color = value;
    }
}