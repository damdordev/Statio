using System;
using System.Collections.Generic;
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

        private int currentStateId = -1;
        private int stateIdToSetAfterEnable = -1;

        public void AddState(string state)
        {
            states.Add(state);
        }
        
        public void RemoveState(string state)
        {
            var stateId = states.IndexOf(state);
            if (stateId == -1) return;
            states.RemoveAt(stateId);
            
            foreach (var parameter in parameters) parameter.NotifyStateRemoved(stateId);
            if (initialStateId == stateId) initialStateId = -1;
            else if (initialStateId > stateId) --initialStateId;
        }

        public void ChangeStateIndex(string state, int newStateId)
        {
            var oldStateId = states.IndexOf(state);
            if (oldStateId == -1) return;

            states.RemoveAt(oldStateId);
            states.Insert(newStateId, state);
            
            foreach (var parameter in parameters) parameter.NotifyStateChanged(oldStateId, newStateId);
            if (initialStateId == oldStateId) initialStateId = newStateId;
            else if(oldStateId < initialStateId && newStateId >= initialStateId) --initialStateId;
            else if(oldStateId >= initialStateId && newStateId <= initialStateId) ++initialStateId;
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
            ChangeState(states.IndexOf(state));
        }

        private void ChangeState(int newStateId)
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
            
                foreach (var parameter in parameters)
                {
                    parameter.LoadValue(newStateId);
                }
            }
            else
            {
                stateIdToSetAfterEnable = newStateId;
            }
        }

        protected virtual void Awake()
        {
            if (currentStateId == -1 && initialStateId != -1)
            {
                ChangeState(initialStateId);
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
            
            var tmp = stateIdToSetAfterEnable;
            stateIdToSetAfterEnable = -1;
            ChangeState(tmp);
        }

        private void AssignStoragesToParameters()
        {
            foreach (var parameter in parameters)
            {
                parameter.Storage = storage;
            }
        }
    }
}