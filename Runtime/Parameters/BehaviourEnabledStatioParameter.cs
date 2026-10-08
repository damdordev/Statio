using System;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("Behaviour/Enabled")]
    public class BehaviourEnabledStatioParameter : StatioParameter<Behaviour, bool>
    {
        protected override bool GetValue(Behaviour target) => target.enabled;
        protected override void SetValue(Behaviour target, bool value) => target.enabled = value;
    }
}
