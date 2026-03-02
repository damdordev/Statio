using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("CanvasGroup/Alpha")]
    public class CanvasGroupAlphaVisualStateParameter : FloatVisualStateParameter<CanvasGroup>
    {
        protected override float GetValue(CanvasGroup target) => target.alpha;

        protected override void SetValue(CanvasGroup target, float value)
            => target.alpha = value;
    }
}