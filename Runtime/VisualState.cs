using System;
using System.Collections.Generic;
using UnityEngine;
using Damdor.Vario;
using Object = UnityEngine.Object;

namespace Damdor.VisualStates
{
    public class VisualState : MonoBehaviour, IVarioStorageSource
    {
        public Vario.VarioStorage Storage => storage;
        public IReadOnlyList<string> States => states;
        public string CurrentState => currentStateId >= 0 && currentStateId < states.Count ? states[currentStateId] : "";

        public string InitialState
        {
            get => initialStateId >= 0 && initialStateId < states.Count ? states[initialStateId] : "";
            set => initialStateId = states.IndexOf(value);
        }
        
        public VisualStateTimescale Timescale
        {
            get => timescale;
            set => timescale = value;
        }
        
        [SerializeField] private List<string> states;
        [SerializeField] private Vario.VarioStorage storage;
        [SerializeReference] private List<IVisualStateParameterLifecycle> parameters = new();
        [SerializeField] private int initialStateId = -1;
        [SerializeField] private List<VisualStateAnimation> animations;
        [SerializeField] private VisualStateTimescale timescale;

        private int currentStateId = -1;
        private int stateIdToSetAfterEnable = -1;
        private bool animateAfterEnable;
        private VisualStateAnimationProgress animationProgress;

        public void AddState(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                VisualStateSettings.NotifyError(this, "Trying to add empty state");
                return;
            }
            if (states.Contains(state))
            {
                VisualStateSettings.NotifyError(this, $"Trying to add existing state: {state}");
                return;
            }
            states.Add(state);
        }
        
        public void RemoveState(string state)
        {
            var removedStateId = states.IndexOf(state);
            if (removedStateId == -1)
            {
                VisualStateSettings.NotifyError(this, $"Trying to remove non-existing state: {state}");
                return;
            }
            states.RemoveAt(removedStateId);
            
            foreach (var parameter in parameters) parameter.NotifyStateRemoved(removedStateId);
            initialStateId = VisualStateHelper.RecalculateStateIdAfterStateRemoved(removedStateId, initialStateId);
        }

        public void ChangeStateId(string state, int newStateId)
        {
            var oldStateId = states.IndexOf(state);
            if (oldStateId == -1)
            {
                VisualStateSettings.NotifyError(this, $"Trying to move non-existing state: {state}");
                return;
            }

            states.RemoveAt(oldStateId);
            states.Insert(newStateId, state);
            
            foreach (var parameter in parameters) parameter.NotifyStateChanged(oldStateId, newStateId);
            initialStateId = VisualStateHelper.RecalculateStateIdAfterStateIdChanged(oldStateId, newStateId, initialStateId);
        }

        public void AddParameter<TComponent, TValue>(VisualStateParameter<TComponent, TValue> parameter)
            where TComponent : Object
        {
            parameters.Add(parameter);
        }
        
        public void AddParameter<TComponent, TValue, TSerializedValue>(VisualStateParameter<TComponent, TSerializedValue, TValue> parameter)
            where TComponent : Object
        {
            parameters.Add(parameter);
        }
        
        public void ChangeState(string state)
        {
            var stateID = states.IndexOf(state);
            if (stateID == -1)
            {
                VisualStateSettings.NotifyError(this, $"Trying to change to non existing state: {state}");
            }

            ChangeState(GetStateId(state), true);
        }
        
        public void ChangeStateImmediately(string state)
        {
            ChangeState(GetStateId(state), false);
        }

        private void ChangeState(int newStateId, bool animate)
        {
            AssignStoragesToParameters();
            
            if (newStateId == -1) return;
            if (newStateId == currentStateId) return;

            if (isActiveAndEnabled)
            {
                var animation = GetAnimation(currentStateId, newStateId);
                currentStateId = newStateId;

                var time = (float)storage.Evaluate(animation.Duration).TotalSeconds;
                if (time <= 0f) animate = false;
                
                if (animate)
                {
                    animationProgress = new VisualStateAnimationProgress
                    {
                        Running = true,
                        CurrentTime = 0f,
                        FullTime = time,
                        TargetState = newStateId,
                        Easing = storage.Evaluate(animation.Easing)
                    };

                    for (var index = 0; index < parameters.Count; index++)
                    {
                        var parameter = parameters[index];
                        try
                        {
                            parameter.SaveSnapshot();
                        }
                        catch (Exception e)
                        {
                            VisualStateSettings.NotifyError(this, $"Error on saving snapshot, parameter {index}:\n{e}");
                        }
                    }
                }
                else
                {
                    animationProgress = new VisualStateAnimationProgress { Running = false };
                    for (var index = 0; index < parameters.Count; index++)
                    {
                        var parameter = parameters[index];
                        try
                        {
                            parameter.LoadValue(newStateId);
                        }
                        catch (Exception e)
                        {
                            VisualStateSettings.NotifyError(this, $"Error on loading value, parameter {index}:\n{e}");
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
            UpdateTime(timescale == VisualStateTimescale.Normal ? Time.deltaTime : Time.unscaledDeltaTime);
        }

        private void AssignStoragesToParameters()
        {
            foreach (var parameter in parameters)
            {
                parameter.Storage = storage;
            }
        }
        
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
                try
                {
                    parameter.LoadValue(stateId);
                }
                catch (Exception e)
                {
                    VisualStateSettings.NotifyError(this, $"Error on loading value, parameter {index}:\n{e}");
                }
            }
        }
        
        private void LoadValuesFromState(int stateId, float percentFromSnapshot)
        {
            for (var index = 0; index < parameters.Count; index++)
            {
                var parameter = parameters[index];
                try
                {
                    parameter.LoadValue(stateId, percentFromSnapshot);
                }
                catch (Exception e)
                {
                    VisualStateSettings.NotifyError(this, $"Error on loading value, parameter {index}:\n{e}");

                }
            }
        }

        private VisualStateAnimation GetAnimation(int initialStateId, int targetStateId)
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

            return new VisualStateAnimation();
        }

        private bool AcceptState(int state, int condition) => condition == -1 || state == condition;

        private int GetStateId(string state)
        {
            var stateId = states.IndexOf(state);
            if (stateId == -1)
            {
                VisualStateSettings.NotifyError(this, $"Trying to get non existing state: {state}");
            }
            return stateId;
        }
        
    }
}