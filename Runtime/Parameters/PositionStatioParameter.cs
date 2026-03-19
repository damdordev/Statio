using Damdor.Vario;
using UnityEngine;

namespace Damdor.Statio
{
    [StatioParameterName("Transform/Position")]
    public class PositionStatioParameter : StatioParameter<Transform, Vector3>
    {
        protected override Vector3 GetValue(Transform target) =>
            ResolveValue(local) ? target.localPosition : target.position;

        protected override void SetValue(Transform target, Vector3 value)
        {
            if (ResolveValue(local)) target.localPosition = value;
            else target.position = value;
        }

        [SerializeField] private VarioValue<bool> local = new() { Value = true };
    }
}