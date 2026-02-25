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
        
        [SerializeField] private List<string> states;
        [SerializeField] private VariableStorage.VariableStorage storage;
        [SerializeReference] private List<IVisualStateParameterLifecycle> parameters;
        [SerializeField] private int initialStateId = -1;

        private int currentStateId = -1;
        private int stateIdToSetAfterEnable = -1;
        
        public void RemoveState(string state)
        {
            var stateIndex = states.IndexOf(state);
            if (stateIndex == -1) return;
            states.RemoveAt(stateIndex);
            foreach (var parameter in parameters) parameter.NotifyStateRemoved(stateIndex);
        }

        public void ChangeStateIndex(string state, int newIndex)
        {
            var oldIndex = states.IndexOf(state);
            if (oldIndex == -1) return;

            states.RemoveAt(oldIndex);
            states.Insert(newIndex, state);
            foreach (var parameter in parameters) parameter.NotifyStateChanged(oldIndex, newIndex);
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