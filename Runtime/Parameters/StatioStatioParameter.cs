using System;
using Damdor.Vario;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("Statio/State")]
    public class StatioStatioParameter : StatioParameter<VisualState, string>
    {
        public VarioValue<float> SwitchMoment
        {
            get => switchMoment;
            set => switchMoment = value;
        }
        
        protected override string GetValue(VisualState target) => target.CurrentState;

        protected override void SetValue(VisualState target, string value)
        {
            if (ResolveValue(animate)) target.ChangeState(value);
            else target.ChangeStateImmediately(value);
        }
        
        protected override string Lerp(string a, string b, float t) => t < ResolveValue(switchMoment) ? a : b;

        [SerializeField] private VarioValue<bool> animate;
        [SerializeField] private VarioValue<float> switchMoment = VarioValue<float>.Raw(0.5f);
        
    }
}