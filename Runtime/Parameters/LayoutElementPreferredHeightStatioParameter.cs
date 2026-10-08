using System;
using UnityEngine.UI;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls LayoutElement.preferredHeight through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("LayoutElement/Preferred Height")]
    public class LayoutElementPreferredHeightStatioParameter : StatioParameter<LayoutElement, float>
    {
        protected override float GetValue(LayoutElement target) => target.preferredHeight;
        protected override void SetValue(LayoutElement target, float value) => target.preferredHeight = value;
    }
}
