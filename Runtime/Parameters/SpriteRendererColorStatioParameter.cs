using System;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls SpriteRenderer.color through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("SpriteRenderer/Color")]
    public class SpriteRendererColorStatioParameter : StatioParameter<SpriteRenderer, Color>
    {
        protected override Color GetValue(SpriteRenderer target) => target.color;
        protected override void SetValue(SpriteRenderer target, Color value) => target.color = value;
    }
}
