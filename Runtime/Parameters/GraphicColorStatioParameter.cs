using System;
using UnityEngine;
using UnityEngine.UI;

namespace Damdor.Statio
{
    [Serializable]
    public class GraphicColorStatioParameter : StatioParameter<Graphic, Color>
    {
        protected override Color GetValue(Graphic target) => target.color;
        protected override void SetValue(Graphic target, Color value) => target.color = value;
    }
}