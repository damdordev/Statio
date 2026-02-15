using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("Transform/Scale")]
    public class ScaleVisualStateParameter : Vector3VisualStateParameter<Transform>
    {
        protected override Vector3 GetValue(Transform target) => target.localScale;
        protected override void SetValue(Transform target, Vector3 value) =>  target.localScale = value;
    }
}