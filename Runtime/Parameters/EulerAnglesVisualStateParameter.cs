using Damdor.Vario;
using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("Transform/EulerAngles")]
    public class EulerAnglesVisualStateParameter : VisualStateParameter<Transform, Vector3>
    {
        protected override Vector3 GetValue(Transform target) =>
            ResolveValue(local) ? target.localEulerAngles : target.eulerAngles;

        protected override void SetValue(Transform target, Vector3 value)
        {
            if(ResolveValue(local)) target.localEulerAngles = value;
            else target.eulerAngles = value;
        }
        
        [SerializeField] private VarioValue<bool> local = new() { Value =  true };
    }
}