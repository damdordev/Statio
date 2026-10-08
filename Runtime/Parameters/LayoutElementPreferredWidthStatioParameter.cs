using System;
using UnityEngine.UI;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls LayoutElement.preferredWidth through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("LayoutElement/Preferred Width")]
    public class LayoutElementPreferredWidthStatioParameter : StatioParameter<LayoutElement, float>
    {
        protected override float GetValue(LayoutElement target) => target.preferredWidth;
        protected override void SetValue(LayoutElement target, float value) => target.preferredWidth = value;
    }
}
