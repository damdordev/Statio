using System;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls CanvasGroup.alpha through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("CanvasGroup/Alpha")]
    public class CanvasGroupAlphaStatioParameter : StatioParameter<CanvasGroup, float>
    {
        protected override float GetValue(CanvasGroup target) => target.alpha;
        protected override void SetValue(CanvasGroup target, float value) => target.alpha = value;
    }
}