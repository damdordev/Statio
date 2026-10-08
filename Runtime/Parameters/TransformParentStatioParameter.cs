using System;
using Damdor.Vario;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("Transform/Parent")]
    public class TransformParentStatioParameter : StatioParameter<Transform, Transform>
    {
        public VarioValue<float> SwitchMoment
        {
            get => switchMoment;
            set => switchMoment = value;
        }
        
        protected override Transform GetValue(Transform target) => target.parent;

        protected override void SetValue(Transform target, Transform value)
        {
            target.SetParent(value, false);
            target.SetAsLastSibling();
        }

        protected override Transform Lerp(Transform a, Transform b, float t) => t >= 0.5f ? a : b;
        [SerializeField] private VarioValue<float> switchMoment = VarioValue<float>.Raw(0.5f);
    }
}