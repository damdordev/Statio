using System;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls Transform.localScale through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("Transform/Scale")]
    public class ScaleStatioParameter : StatioParameter<Transform, Vector3>
    {
        protected override Vector3 GetValue(Transform target) => target.localScale;
        protected override void SetValue(Transform target, Vector3 value) =>  target.localScale = value;
    }
}