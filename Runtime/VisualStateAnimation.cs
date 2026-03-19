using System;
using Damdor.Foundation;
using Damdor.Vario;
using UnityEngine;

namespace Damdor.VisualStates
{
    [Serializable]
    public struct VisualStateAnimation
    {
        public int InitialStateId;
        public int TargetStateId;
        public VarioValue<float> Duration;
        public VarioValue<AnimationCurve> Easing;
    }
}