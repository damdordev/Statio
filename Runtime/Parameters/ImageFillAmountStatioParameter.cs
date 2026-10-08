using System;
using UnityEngine.UI;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("Image/Fill Amount")]
    public class ImageFillAmountStatioParameter : StatioParameter<Image, float>
    {
        protected override float GetValue(Image target) => target.fillAmount;
        protected override void SetValue(Image target, float value) => target.fillAmount = value;
    }
}
