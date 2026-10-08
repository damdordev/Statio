using System;
using UnityEngine;
using UnityEngine.UI;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls Graphic.color through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("Graphic/Color")]
    public class GraphicColorStatioParameter : StatioParameter<Graphic, Color>
    {
        protected override Color GetValue(Graphic target) => target.color;
        protected override void SetValue(Graphic target, Color value) => target.color = value;
    }
}