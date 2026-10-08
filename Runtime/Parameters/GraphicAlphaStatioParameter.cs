using System;
using UnityEngine.UI;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls Graphic.color.a through visual state values.
    /// </summary>
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
