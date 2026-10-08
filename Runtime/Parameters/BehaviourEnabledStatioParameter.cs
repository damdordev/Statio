using System;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls Behaviour.enabled through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("Behaviour/Enabled")]
    public class BehaviourEnabledStatioParameter : StatioParameter<Behaviour, bool>
    {
        protected override bool GetValue(Behaviour target) => target.enabled;
        protected override void SetValue(Behaviour target, bool value) => target.enabled = value;
    }
}
