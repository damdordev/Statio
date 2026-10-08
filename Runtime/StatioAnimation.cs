using System;
using Damdor.Vario;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Defines an animation rule between two state indices.
    /// </summary>
    [Serializable]
    public struct StatioAnimation
    {
        /// <summary>
        /// The source state index, or -1 to match any source state.
        /// </summary>
        public int InitialStateId;
        /// <summary>
        /// The destination state index, or -1 to match any destination state.
        /// </summary>
        public int TargetStateId;
        /// <summary>
        /// Duration in seconds, evaluated when the transition starts. Nonpositive values apply the state immediately.
        /// </summary>
        public VarioValue<float> Duration;
        /// <summary>
        /// The curve evaluated at normalized elapsed time. A null curve uses linear progress.
        /// </summary>
        public VarioValue<AnimationCurve> Easing;
    }
}