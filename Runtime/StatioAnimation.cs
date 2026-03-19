using System;
using Damdor.Foundation;
using Damdor.Vario;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    public struct StatioAnimation
    {
        public int InitialStateId;
        public int TargetStateId;
        public VarioValue<float> Duration;
        public VarioValue<AnimationCurve> Easing;
    }
}