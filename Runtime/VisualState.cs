using System;
using System.Collections.Generic;
using Damdor.Foundation;
using UnityEngine;
using Damdor.VariableStorage;
using Object = UnityEngine.Object;

namespace Damdor.VisualStates
{
    public class VisualState : MonoBehaviour, IVariableStorageSource
    {
        public VariableStorage.VariableStorage Storage => storage;
        public IReadOnlyList<string> States => states;
        public string CurrentState => currentStateId >= 0 && currentStateId < states.Count ? states[currentStateId] : "";

        public string InitialState
        {
            get => initialStateId >= 0 && initialStateId < states.Count ? states[initialStateId] : "";
            set => initialStateId = states.IndexOf(value);
        }
        
        [SerializeField] private List<string> states;
        [SerializeField] private VariableStorage.VariableStorage storage;
        [SerializeReference] private List<IVisualStateParameterLifecycle> parameters;
        [SerializeField] private int initialStateId = -1;
        [SerializeField] private StorageValue<TimeSpan, SerializableTimeSpan> animationTime;

        private int currentStateId = -1;
        private int stateIdToSetAfterEnable = -1;
        private bool animateAfterEnable;
        private VisualStateAnimationProgress animationProgress;

        public void AddState(string state)
        {
            states.Add(state);
        }
        
        public void RemoveState(string state)
        {
            var removedStateId = states.IndexOf(state);
            if (removedStateId == -1) return;
            states.RemoveAt(removedStateId);
            
            foreach (var parameter in parameters) parameter.NotifyStateRemoved(removedStateId);
            initialStateId = VisualStateHelper.RecalculateStateIdAfterStateRemoved(removedStateId, initialStateId);
        }

        public void ChangeStateId(string state, int newStateId)
        {
            var oldStateId = states.IndexOf(state);
            if (oldStateId == -1) return;

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
            ChangeState(states.IndexOf(state), true);
        }
        
        public void ChangeStateImmediately(string state)
        {
            ChangeState(states.IndexOf(state), false);
        }

        private void ChangeState(int newStateId, bool animate)
        {
            AssignStoragesToParameters();
            
            if (newStateId == -1)
            {
                // TODO: error
                return;
            }

            if (newStateId == currentStateId) return;

            if (isActiveAndEnabled)
            {
                currentStateId = newStateId;

                var time = (float)storage.Evaluate(animationTime).TotalSeconds;
                if (time <= 0f) animate = false;
                
                if (animate)
                {
                    animationProgress = new VisualStateAnimationProgress
                    {
                        Running = true,
                        CurrentTime = 0f,
                        FullTime = time,
                        TargetState = newStateId
                    };
                    
                    foreach (var parameter in parameters) parameter.SaveSnapshot();
                }
                else
                {
                    animationProgress = new VisualStateAnimationProgress { Running = false };
                    foreach (var parameter in parameters) parameter.LoadValue(newStateId);
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
            UpdateTime(Time.deltaTime);
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
            foreach (var parameter in parameters)
            {
                parameter.LoadValue(stateId);
            }
        }
        
        private void LoadValuesFromState(int stateId, float percentFromSnapshot)
        {
            foreach (var parameter in parameters)
            {
                parameter.LoadValue(stateId, percentFromSnapshot);
            }
        }
        
    }
}