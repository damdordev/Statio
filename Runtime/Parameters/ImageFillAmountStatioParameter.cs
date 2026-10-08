using System;
using UnityEngine.UI;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls Image.fillAmount through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("Image/Fill Amount")]
    public class ImageFillAmountStatioParameter : StatioParameter<Image, float>
    {
        protected override float GetValue(Image target) => target.fillAmount;
        protected override void SetValue(Image target, float value) => target.fillAmount = value;
    }
}
