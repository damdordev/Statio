using System;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls GameObject.activeSelf through visual state values. Keeps the target active while the interpolation factor is below 0.99 if either endpoint is active.
    /// </summary>
    [Serializable]
    [StatioParameter("GameObject/Active")]
    public class GameObjectActivityStatioParameter : StatioParameter<GameObject, bool>
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