using UnityEngine;
using UnityEngine.UI;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("Image/Color")]
    public class ImageColorVisualStateParameter : ColorVisualStateParameter<Image>
    {
        protected override Color GetValue(Image target) => target.color;
        protected override void SetValue(Image target, Color value) => target.color = value;
    }
}