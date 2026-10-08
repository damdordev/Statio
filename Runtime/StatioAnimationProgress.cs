using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Stores the runtime progress of a state transition.
    /// </summary>
    public struct StatioAnimationProgress
    {
        /// <summary>
        /// Whether the animation is currently running.
        /// </summary>
        public bool Running;
        /// <summary>
        /// The destination state index.
        /// </summary>
        public int TargetState;
        /// <summary>
        /// The duration in seconds captured when the transition starts.
        /// </summary>
        public float FullTime;
        /// <summary>
        /// The accumulated elapsed time in seconds.
        /// </summary>
        public float CurrentTime;
        /// <summary>
        /// The easing curve captured at transition start, or null for linear progress.
        /// </summary>
        public AnimationCurve Easing;
    }
}