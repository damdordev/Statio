using Damdor.Vario;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Interface for a visual state parameter that controls a specific property of a component.
    /// Allows configuring values for different states and a default fallback.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Unity Object (Component) being controlled.</typeparam>
    /// <typeparam name="TValue">The type of the value being applied to the component.</typeparam>
    /// <example>
    /// <code>
    /// // Example usage for controlling a CanvasGroup's alpha
    /// public class CanvasGroupAlphaParameter : StatioParameter&lt;CanvasGroup, float&gt;
    /// {
    ///     protected override float GetValue(CanvasGroup target) => target.alpha;
    ///     protected override void SetValue(CanvasGroup target, float value) => target.alpha = value;
    ///     protected override float Lerp(float a, float b, float t) => Mathf.Lerp(a, b, t);
    /// }
    /// 
    /// // Accessing via interface
    /// VisualState state = GetVisualState();
    /// 
    /// var param = new CanvasGroupAlphaParameter();
    /// param.Target = new VarioValue&lt;CanvasGroup&gt; { Value = myCanvasGroup, Source = ValueSource.Raw };
    /// param.DefaultValue = new VarioValue&lt;float&gt; { Value = 1.0f, Source = ValueSource.Raw };
    /// param.SetValue(stateId: 1, new VarioValue&lt;float&gt; { Value = 0.0f, Source = ValueSource.Raw });
    ///
    /// state.AddParameter(param);
    /// </code>
    /// </example>
    public interface IStatioParameter<TComponent, TValue>
        where TComponent : Object
    {
        /// <summary>
        /// Gets or sets the target component to be modified by this parameter.
        /// Can be a direct reference (Raw) or resolved via VarioStorage.
        /// </summary>
        VarioValue<TComponent> Target { get; set; }
        
        /// <summary>
        /// Gets or sets the default value to apply when no specific state override is active.
        /// </summary>
        VarioValue<TValue> DefaultValue { get; set; }
        
        /// <summary>
        /// Sets an override value for a specific state ID.
        /// </summary>
        /// <param name="stateId">The zero-based index of the state.</param>
        /// <param name="value">The value to apply when this state is active.</param>
        void SetValue(int stateId, VarioValue<TValue> value);
        
        /// <summary>
        /// Retrieves the configured value for a specific state ID.
        /// Returns the default value if no override exists for the given state.
        /// </summary>
        /// <param name="stateId">The zero-based index of the state.</param>
        /// <returns>The configured value for the state.</returns>
        VarioValue<TValue> GetValue(int stateId);
        
        /// <summary>
        /// Checks if a specific override exists for the given state ID.
        /// </summary>
        /// <param name="stateId">The zero-based index of the state.</param>
        /// <returns>True if an override is defined; otherwise, false.</returns>
        bool HasOverride(int stateId);
        
        /// <summary>
        /// Removes the override for the specified state ID, reverting it to the default value.
        /// </summary>
        /// <param name="stateId">The zero-based index of the state to clear.</param>
        void RemoveOverride(int stateId);
    }
}