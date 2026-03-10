using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("CanvasGroup/Alpha")]
    public class CanvasGroupAlphaVisualStateParameter : VisualStateParameter<CanvasGroup, float>
    {
        protected override float GetValue(CanvasGroup target) => target.alpha;
        protected override void SetValue(CanvasGroup target, float value) => target.alpha = value;
    }
}