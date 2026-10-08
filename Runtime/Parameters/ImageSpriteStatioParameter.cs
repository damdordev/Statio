using System;
using UnityEngine;
using UnityEngine.UI;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls Image.sprite through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("Image/Sprite")]
    public class ImageSpriteStatioParameter : StatioParameter<Image, Sprite>
    {
        protected override Sprite GetValue(Image target) => target.sprite;
        protected override void SetValue(Image target, Sprite value) => target.sprite = value;
    }
}
