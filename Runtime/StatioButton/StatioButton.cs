using System;
using UnityEngine;
using UnityEngine.UI;

namespace Damdor.Statio
{
    /// <summary>
    /// Defines states for <see cref="StatioButton"/> to transition to.
    /// </summary>
    [Serializable]
    public class StatioButtonStates
    {
        /// <summary>
        /// The VisualState component that will be controlled by the button.
        /// </summary>
        public VisualState visualState;
        
        /// <summary>
        /// The name of the state for when the button is in its normal, highlighted, or selected state.
        /// </summary>
        public string NormalState = "normal";
        
        /// <summary>
        /// The name of the state for when the button is disabled.
        /// </summary>
        public string DisabledState = "disabled";
        
        /// <summary>
        /// The name of the state for when the button is pressed.
        /// </summary>
        public string PressedState = "pressed";
    }
    
    /// <summary>
    /// A UI Button that integrates with a <see cref="VisualState"/> component to drive state transitions.
    /// It also calls the base Button transition, so configured Unity transitions can run alongside Statio state changes.
    /// </summary>
    [AddComponentMenu("UI/Statio Button", 31)]
    public class StatioButton : Button
    {
        [SerializeField] private StatioButtonStates visualState;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            if (visualState == null || visualState.visualState == null)
                return;

            var targetState = state switch
            {
                SelectionState.Normal or SelectionState.Highlighted or SelectionState.Selected => visualState
                    .NormalState,
                SelectionState.Pressed => visualState.PressedState,
                SelectionState.Disabled => visualState.DisabledState,
                _ => null
            };

            if (string.IsNullOrEmpty(targetState)) return;
            if (instant)
            {
                visualState.visualState.ChangeStateImmediately(targetState);
            }
            else
            {
                visualState.visualState.ChangeState(targetState);
            }
        }
    }
}