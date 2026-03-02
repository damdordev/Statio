using UnityEngine;

namespace Damdor.VisualStates
{
    [VisualParameterTypeName("GameObject/Active")]
    public class GameObjectActivityVisualStateParameter : BoolVisualStateParameter<GameObject>
    {
        protected override bool GetValue(GameObject target) => target.activeSelf;
        protected override void SetValue(GameObject target, bool value) => target.SetActive(value);
        
        protected override bool Lerp(bool value1, bool value2, float t)
        {
            if((value1 || value2) && t < 0.99) return true;
            return base.Lerp(value1, value2, t);
        }
    }
}