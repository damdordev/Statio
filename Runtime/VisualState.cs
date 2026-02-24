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
        public string CurrentState => states[currentState];

        [SerializeField] private List<string> states;
        [SerializeField] private VariableStorage.VariableStorage storage;
        [SerializeReference] private List<IVisualStateParameterLifecycle> parameters;

        private int currentState;
        
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
            ChangeStateImmediately(state);
        }

        public void ChangeStateImmediately(string state)
        {
            var newStateId = states.IndexOf(state);
            if (newStateId == currentState) return;
            currentState = newStateId;
            
            foreach (var parameter in parameters)
            {
                parameter.LoadValue(newStateId);
            }
        }

        protected virtual void Awake()
        {
            AssignStoragesToParameters();
        }

        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;
            AssignStoragesToParameters();
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