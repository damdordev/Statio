namespace Damdor.Statio
{
    /// <summary>
    /// Defines the lifecycle methods for a visual state parameter, handling value application, snapshots, and state management.
    /// </summary>
    public interface IStatioParameterLifecycle
    {
        /// <summary>
        /// Gets or sets the primary variable storage used for resolving values.
        /// </summary>
        Vario.VarioStorage Storage { get; set; }
        
        /// <summary>
        /// Applies the default value to the target component.
        /// </summary>
        void LoadDefaultValue();
        
        /// <summary>
        /// Applies the value associated with the specified state ID to the target component.
        /// </summary>
        /// <param name="stateId">The identifier of the state to load.</param>
        void LoadValue(int stateId);
        
        /// <summary>
        /// Applies an interpolated value between the saved snapshot and the target state value.
        /// </summary>
        /// <param name="stateId">The identifier of the target state.</param>
        /// <param name="percentFromSnapshot">The eased interpolation factor, where 0 is the snapshot and 1 is the target state value. Easing may produce values outside this range.</param>
        void LoadValue(int stateId, float percentFromSnapshot);
        
        /// <summary>
        /// Captures the current value of the target component as a snapshot for interpolation.
        /// </summary>
        void SaveSnapshot();

        /// <summary>
        /// Saves the current value of the target component as the default value.
        /// </summary>
        void SaveCurrentValueToDefaultValue();

        /// <summary>
        /// Saves the current value of the target component to the specified state ID.
        /// </summary>
        /// <param name="stateId">The identifier of the state to save to.</param>
        void SaveCurrentValueToState(int stateId);
        
        /// <summary>
        /// Updates internal data structures when a state is removed, shifting subsequent state IDs accordingly.
        /// </summary>
        /// <param name="stateId">The identifier of the removed state.</param>
        void NotifyStateRemoved(int stateId);
        
        /// <summary>
        /// Updates internal data structures when a state is moved or reordered.
        /// </summary>
        /// <param name="oldIndex">The original index/ID of the state.</param>
        /// <param name="newIndex">The new index/ID of the state.</param>
        void NotifyStateChanged(int oldIndex, int newIndex);
    }
}