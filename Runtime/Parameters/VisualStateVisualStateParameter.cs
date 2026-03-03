using Damdor.Vario;
using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("VisualState/State")]
    public class VisualStateVisualStateParameter : StringVisualStateParameter<VisualState>
    {
        protected override string GetValue(VisualState target) => target.CurrentState;

        protected override void SetValue(VisualState target, string value)
        {
            if (ResolveValue(animate)) target.ChangeState(value);
            else target.ChangeStateImmediately(value);
        }

        [SerializeField] private VarioValue<bool> animate;
    }
}