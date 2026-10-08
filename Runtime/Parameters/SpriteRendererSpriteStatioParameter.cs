using System;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls SpriteRenderer.sprite through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("SpriteRenderer/Sprite")]
    public class SpriteRendererSpriteStatioParameter : StatioParameter<SpriteRenderer, Sprite>
    {
        protected override Sprite GetValue(SpriteRenderer target) => target.sprite;
        protected override void SetValue(SpriteRenderer target, Sprite value) => target.sprite = value;
    }
}
