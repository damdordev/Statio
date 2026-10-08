using System;
using System.Collections.Generic;
using UnityEngine;
using Damdor.Vario;
using Object = UnityEngine.Object;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls named visual states and animates configured parameters between them.
    /// </summary>
    public class VisualState : MonoBehaviour, IVarioStorageSource
    {
        /// <summary>
        /// Gets the storage used to evaluate parameter targets, values, duration, and easing.
        /// </summary>
        public VarioStorage Storage => storage;
        /// <summary>
        /// Gets state names in index order; state IDs are zero-based indices in this list.
        /// </summary>
        public IReadOnlyList<string> States => states;
        /// <summary>
        /// Gets the current destination state name, or an empty string when no state is selected.
        /// </summary>
        /// <remarks>During a transition this reports the destination, before its values have finished animating.</remarks>
        public string CurrentState => currentStateId >= 0 && currentStateId < states.Count ? states[currentStateId] : "";

        /// <summary>
        /// Gets or sets the state applied without animation in Awake. An unknown name clears the selection.
        /// </summary>
        public string InitialState
        {
            get => initialStateId >= 0 && initialStateId < states.Count ? states[initialStateId] : "";
            set => initialStateId = states.IndexOf(value);
        }
        
        /// <summary>
        /// Gets or sets whether automatic updates use scaled or unscaled delta time.
        /// </summary>
        public StatioTimescale Timescale
        {
            get => timescale;
            set => timescale = value;
        }

        [SerializeField] private List<string> states = new();
        [SerializeField] private VarioStorage storage;
        [SerializeReference] private List<IStatioParameterLifecycle> parameters = new();
        [SerializeField] private int initialStateId = -1;
        [SerializeField] private List<StatioAnimation> animations = new();
        [SerializeField] private StatioTimescale timescale;

        private int currentStateId = -1;
        private int stateIdToSetAfterEnable = -1;
        private bool animateAfterEnable;
        private StatioAnimationProgress animationProgress;

        /// <summary>
        /// Adds a nonempty, unique state name.
        /// </summary>
        /// <param name="state">The case-sensitive state name.</param>
        /// <returns>Index of the new added state</returns>
        /// <remarks>Empty or duplicate names are reported through StatioSettings and ignored.</remarks>
        public int AddState(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                StatioSettings.NotifyError(this, "Trying to add empty state");
                return -1;
            }
            if (states.Contains(state))
            {
                StatioSettings.NotifyError(this, $"Trying to add existing state: {state}");
                return - 1;
            }
            states.Add(state);
            return states.Count - 1;
        }
        
        /// <summary>
        /// Removes a state and remaps parameter overrides and tracked state indices.
        /// </summary>
        /// <param name="state">The name to remove.</param>
        /// <remarks>Unknown names are reported and ignored.</remarks>
        public void RemoveState(string state)
        {
            var removedStateId = states.IndexOf(state);
            if (removedStateId == -1)
            {
                StatioSettings.NotifyError(this, $"Trying to remove non-existing state: {state}");
                return;
            }
            states.RemoveAt(removedStateId);
            
            foreach (var parameter in parameters) 
            {
                if (parameter != null) parameter.NotifyStateRemoved(removedStateId);
            }

            for (var index = 0; index < animations.Count; index++)
            {
                animations[index] = new StatioAnimation
                {
                    Duration = animations[index].Duration,
                    Easing = animations[index].Easing,
                    InitialStateId = StatioInternalHelper.RecalculateStateIdAfterStateRemoved(removedStateId, animations[index].InitialStateId),
                    TargetStateId = StatioInternalHelper.RecalculateStateIdAfterStateRemoved(removedStateId, animations[index].TargetStateId),
                };
            }

            initialStateId = StatioInternalHelper.RecalculateStateIdAfterStateRemoved(removedStateId, initialStateId);
            currentStateId = StatioInternalHelper.RecalculateStateIdAfterStateRemoved(removedStateId, currentStateId);
            animationProgress.TargetState = StatioInternalHelper.RecalculateStateIdAfterStateRemoved(removedStateId, animationProgress.TargetState);
            stateIdToSetAfterEnable = StatioInternalHelper.RecalculateStateIdAfterStateRemoved(removedStateId, stateIdToSetAfterEnable);
        }

        /// <summary>
        /// Moves a state to a new index and remaps parameter overrides and tracked state indices.
        /// </summary>
        /// <param name="state">The name of the state to move.</param>
        /// <param name="newStateId">The new zero-based index in States.</param>
        /// <remarks>Invalid names or indices are reported and ignored.</remarks>
        public void ChangeStateId(string state, int newStateId)
        {
            var oldStateId = states.IndexOf(state);
            if (oldStateId == -1)
            {
                StatioSettings.NotifyError(this, $"Trying to move non-existing state: {state}");
                return;
            }
            
            if(newStateId < 0 || newStateId >= states.Count)
            {
                StatioSettings.NotifyError(this, $"Trying to move state to invalid index: {newStateId}");
                return;
            }

            states.RemoveAt(oldStateId);
            states.Insert(newStateId, state);
            
            foreach (var parameter in parameters) 
            {
                if (parameter != null) parameter.NotifyStateChanged(oldStateId, newStateId);
            }
            
            for (var index = 0; index < animations.Count; index++)
            {
                animations[index] = new StatioAnimation
                {
                    Duration = animations[index].Duration,
                    Easing = animations[index].Easing,
                    InitialStateId = StatioInternalHelper.RecalculateStateIdAfterStateIdChanged(oldStateId, newStateId, animations[index].InitialStateId),
                    TargetStateId = StatioInternalHelper.RecalculateStateIdAfterStateIdChanged(oldStateId, newStateId, animations[index].TargetStateId),
                };
            }
            
            initialStateId = StatioInternalHelper.RecalculateStateIdAfterStateIdChanged(oldStateId, newStateId, initialStateId);
            currentStateId = StatioInternalHelper.RecalculateStateIdAfterStateIdChanged(oldStateId, newStateId, currentStateId);
            animationProgress.TargetState = StatioInternalHelper.RecalculateStateIdAfterStateIdChanged(oldStateId, newStateId, animationProgress.TargetState);
            stateIdToSetAfterEnable = StatioInternalHelper.RecalculateStateIdAfterStateIdChanged(oldStateId, newStateId, stateIdToSetAfterEnable);
        }

        /// <summary>
        /// Adds a parameter to be controlled by this component.
        /// </summary>
        /// <typeparam name="TComponent">The Unity object type controlled by the parameter.</typeparam>
        /// <typeparam name="TValue">The controlled value type.</typeparam>
        /// <param name="parameter">The parameter to add.</param>
        /// <remarks>Adding a parameter does not apply the current state. Storage is assigned on state changes and validation.</remarks>
        public void AddParameter<TComponent, TValue>(StatioParameter<TComponent, TValue> parameter)
            where TComponent : Object
        {
            parameters.Add(parameter);
        }
        
        /// <summary>
        /// Requests a named state using the first matching animation rule, or applies it immediately if no positive duration is configured.
        /// </summary>
        /// <param name="state">The case-sensitive destination name.</param>
        /// <remarks>Unknown names are reported and ignored. While inactive or disabled, only the latest request is retained until OnEnable.</remarks>
        public void ChangeState(string state)
        {
            var stateId = GetStateId(state);
            if (stateId == -1) return;

            ChangeState(stateId, true);
        }
        
        /// <summary>
        /// Requests a named state without animation, completing an active transition even when its destination is already current.
        /// </summary>
        /// <param name="state">The case-sensitive destination name.</param>
        /// <remarks>Unknown names are reported and ignored. While inactive or disabled, the request is deferred until OnEnable.</remarks>
        public void ChangeStateImmediately(string state)
        {
            var stateId = GetStateId(state);
            if (stateId == -1) return;
            
            ChangeState(stateId, false);
        }

        /// <summary>
        /// Requests a state by index, optionally animating from a snapshot of current target values.
        /// </summary>
        /// <param name="newStateId">The destination index in States.</param>
        /// <param name="animate">Whether to animate when a matching rule has a positive duration.</param>
        /// <remarks>Invalid indices are reported and ignored. Repeated animated requests for the current destination do nothing. While inactive or disabled, only the latest request is retained.</remarks>
        public void ChangeState(int newStateId, bool animate)
        {
            AssignStoragesToParameters();

            if (newStateId < 0 || newStateId >= states.Count)
            {
                StatioSettings.NotifyError(this, $"Trying to change to non existing state: {newStateId}");
                return;
            }
            if (newStateId == currentStateId && !(animationProgress.Running && !animate)) return;

            if (isActiveAndEnabled)
            {
                var animation = GetAnimation(currentStateId, newStateId);
                currentStateId = newStateId;

                var time = animation.Duration.Evaluate(storage);
                if (time <= 0f) animate = false;
                
                if (animate)
                {
                    animationProgress = new StatioAnimationProgress
                    {
                        Running = true,
                        CurrentTime = 0f,
                        FullTime = time,
                        TargetState = newStateId,
                        Easing = animation.Easing.Evaluate(storage)
                    };

                    for (var index = 0; index < parameters.Count; index++)
                    {
                        var parameter = parameters[index];
                        if (parameter == null) continue;
                        try
                        {
                            parameter.SaveSnapshot();
                        }
                        catch (Exception e)
                        {
                            StatioSettings.NotifyError(this, $"Error on saving snapshot, parameter {index}:\n{e}");
                        }
                    }
                }
                else
                {
                    animationProgress = new StatioAnimationProgress { Running = false };
                    for (var index = 0; index < parameters.Count; index++)
                    {
                        var parameter = parameters[index];
                        if (parameter == null) continue;
                        try
                        {
                            parameter.LoadValue(newStateId);
                        }
                        catch (Exception e)
                        {
                            StatioSettings.NotifyError(this, $"Error on loading value, parameter {index}:\n{e}");
                        }
                    }
                }
            }
            else
            {
                stateIdToSetAfterEnable = newStateId;
                animateAfterEnable = animate;
            }
        }

        protected virtual void Awake()
        {
            if (currentStateId == -1 && initialStateId != -1)
            {
                ChangeState(initialStateId, false);
            }
        }

        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;
            AssignStoragesToParameters();
        }

        protected void OnEnable()
        {
            if (stateIdToSetAfterEnable == -1) return;
            
            var tmpStateIdToSetAfterEnable = stateIdToSetAfterEnable;
            var tmpAnimateAfterEnable = animateAfterEnable;
            stateIdToSetAfterEnable = -1;
            animateAfterEnable = false;
            ChangeState(tmpStateIdToSetAfterEnable, tmpAnimateAfterEnable);
        }

        protected virtual void Update()
        {
            UpdateTime(timescale == StatioTimescale.Normal ? Time.deltaTime : Time.unscaledDeltaTime);
        }

        private void AssignStoragesToParameters()
        {
            foreach (var parameter in parameters)
            {
                if (parameter != null) parameter.Storage = storage;
            }
        }
        
        /// <summary>
        /// Advances an active animation and applies its parameter values.
        /// </summary>
        /// <param name="dt">Elapsed time in seconds to add to the animation.</param>
        /// <remarks>Update calls this automatically. Manual calls advance the animation in addition to automatic updates. Completion applies exact destination values.</remarks>
        public void UpdateTime(float dt)
        {
            if (!animationProgress.Running) return;

            animationProgress.CurrentTime += dt;
            if (animationProgress.CurrentTime < animationProgress.FullTime)
            {
                var t = animationProgress.CurrentTime / animationProgress.FullTime;
                if (animationProgress.Easing != null) t = animationProgress.Easing.Evaluate(t);
                LoadValuesFromState(animationProgress.TargetState, t);
            }
            else
            {
                animationProgress.Running = false;
                LoadValuesFromState(animationProgress.TargetState);
            }
        }
        
        private void LoadValuesFromState(int stateId)
        {
            for (var index = 0; index < parameters.Count; index++)
            {
                var parameter = parameters[index];
                if (parameter == null) continue;
                try
                {
                    parameter.LoadValue(stateId);
                }
                catch (Exception e)
                {
                    StatioSettings.NotifyError(this, $"Error on loading value, parameter {index}:\n{e}");
                }
            }
        }
        
        private void LoadValuesFromState(int stateId, float percentFromSnapshot)
        {
            for (var index = 0; index < parameters.Count; index++)
            {
                var parameter = parameters[index];
                if (parameter == null) continue;
                try
                {
                    parameter.LoadValue(stateId, percentFromSnapshot);
                }
                catch (Exception e)
                {
                    StatioSettings.NotifyError(this, $"Error on loading value, parameter {index}:\n{e}");

                }
            }
        }

        private StatioAnimation GetAnimation(int initialStateId, int targetStateId)
        {
            foreach (var a in animations)
            {
                if (a.InitialStateId == -1 || a.TargetStateId == -1) continue;
                if (!AcceptState(initialStateId, a.InitialStateId)) continue;
                if (!AcceptState(targetStateId, a.TargetStateId)) continue;

                return a;
            }

            foreach (var a in animations)
            {
                if (!AcceptState(initialStateId, a.InitialStateId)) continue;
                if (!AcceptState(targetStateId, a.TargetStateId)) continue;

                return a;
            }

            return new StatioAnimation();
        }

        private bool AcceptState(int state, int condition) => condition == -1 || state == condition;

        private int GetStateId(string state)
        {
            var stateId = states.IndexOf(state);
            if (stateId == -1)
            {
                StatioSettings.NotifyError(this, $"Trying to get non existing state: {state}");
            }
            return stateId;
        }
        
    }
}