using Damdor.VariableStorage;
using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("Transform/Position")]
    public class PositionVisualStateParameter : Vector3VisualStateParameter<Transform>
    {
        [SerializeField] private StorageValue<bool> local;
        
        protected override Vector3 GetValue(Transform target) => target.position;
        protected override void SetValue(Transform target, Vector3 value) => target.position = value;
    }
}