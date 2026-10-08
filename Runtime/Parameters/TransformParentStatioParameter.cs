using System;
using Damdor.Vario;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls Transform.parent through visual state values. Reparents with worldPositionStays set to false and moves the target to the last sibling.
    /// </summary>
    [Serializable]
    [StatioParameter("Transform/Parent")]
    public class TransformParentStatioParameter : StatioParameter<Transform, Transform>
    {
        /// <summary>
        /// Gets or sets the evaluated interpolation threshold at which the snapshot switches to the destination. Defaults to 0.5.
        /// </summary>
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

        protected override Transform Lerp(Transform a, Transform b, float t) => t < ResolveValue(switchMoment) ? a : b;
        [SerializeField] private VarioValue<float> switchMoment = VarioValue<float>.Raw(0.5f);
    }
}