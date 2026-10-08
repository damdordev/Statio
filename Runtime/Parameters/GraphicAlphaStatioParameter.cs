using System;
using UnityEngine.UI;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("Graphic/Alpha")]
    public class GraphicAlphaStatioParameter : StatioParameter<Graphic, float>
    {
        protected override float GetValue(Graphic target) => target.color.a;

        protected override void SetValue(Graphic target, float value)
        {
            var color = target.color;
            color.a = value;
            target.color = color;
        }
    }
}
