using System;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("SpriteRenderer/Color")]
    public class SpriteRendererColorStatioParameter : StatioParameter<SpriteRenderer, Color>
    {
        protected override Color GetValue(SpriteRenderer target) => target.color;
        protected override void SetValue(SpriteRenderer target, Color value) => target.color = value;
    }
}
