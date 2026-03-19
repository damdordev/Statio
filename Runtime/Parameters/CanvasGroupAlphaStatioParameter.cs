using UnityEngine;

namespace Damdor.Statio
{
    [StatioParameterName("CanvasGroup/Alpha")]
    public class CanvasGroupAlphaStatioParameter : StatioParameter<CanvasGroup, float>
    {
        protected override float GetValue(CanvasGroup target) => target.alpha;
        protected override void SetValue(CanvasGroup target, float value) => target.alpha = value;
    }
}