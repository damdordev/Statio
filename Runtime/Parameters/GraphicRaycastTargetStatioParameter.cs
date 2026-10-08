using System;
using UnityEngine.UI;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("Graphic/Raycast Target")]
    public class GraphicRaycastTargetStatioParameter : StatioParameter<Graphic, bool>
    {
        protected override bool GetValue(Graphic target) => target.raycastTarget;
        protected override void SetValue(Graphic target, bool value) => target.raycastTarget = value;
    }
}
