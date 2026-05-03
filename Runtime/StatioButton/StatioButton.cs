using System;
using UnityEngine;
using UnityEngine.Serialization;
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
    /// Instead of using fixed transitions like color tints or sprite swaps, it changes the state of a target <see cref="VisualState"/>.
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

            string targetState = null;

            switch (state)
            {
                case SelectionState.Normal:
                case SelectionState.Highlighted:
                case SelectionState.Selected:
                    targetState = visualState.NormalState;
                    break;
                case SelectionState.Pressed:
                    targetState = visualState.PressedState;
                    break;
                case SelectionState.Disabled:
                    targetState = visualState.DisabledState;
                    break;
            }

            if (!string.IsNullOrEmpty(targetState))
            {
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
}