using System;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("CanvasGroup/Alpha")]
    public class CanvasGroupAlphaStatioParameter : StatioParameter<CanvasGroup, float>
    {
        protected override float GetValue(CanvasGroup target) => target.alpha;
        protected override void SetValue(CanvasGroup target, float value) => target.alpha = value;
    }
}