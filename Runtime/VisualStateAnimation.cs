using System;
using Damdor.Foundation;
using Damdor.VariableStorage;
using UnityEngine;

namespace Damdor.VisualStates
{
    [Serializable]
    public struct VisualStateAnimation
    {
        public int InitialStateId;
        public int TargetStateId;
        public StorageValue<TimeSpan, SerializableTimeSpan> Duration;
        public StorageValue<AnimationCurve> Easing;
    }
}