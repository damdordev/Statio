using System;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("SpriteRenderer/Sprite")]
    public class SpriteRendererSpriteStatioParameter : StatioParameter<SpriteRenderer, Sprite>
    {
        protected override Sprite GetValue(SpriteRenderer target) => target.sprite;
        protected override void SetValue(SpriteRenderer target, Sprite value) => target.sprite = value;
    }
}
