using UnityEngine;

namespace Damdor.Statio
{
    public struct StatioAnimationProgress
    {
        public bool Running;
        public int TargetState;
        public float FullTime;
        public float CurrentTime;
        public AnimationCurve Easing;
    }
}