using UnityEngine;

namespace Damdor.VisualStates
{
    public struct VisualStateAnimationProgress
    {
        public bool Running;
        public int TargetState;
        public float FullTime;
        public float CurrentTime;
        public AnimationCurve Easing;
    }
}