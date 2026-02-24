using System.Collections.Generic;
using UnityEngine;
using Damdor.VariableStorage;

namespace Damdor.VisualStates
{
    public class VisualState : MonoBehaviour, IVariableStorageSource
    {
        public VariableStorage.VariableStorage Storage => storage;
        public IReadOnlyList<string> States => states;

        [SerializeField] private List<string> states;
        [SerializeField] private VariableStorage.VariableStorage storage;
        [SerializeReference] private List<IVisualStateParameterLifecycle> parameters;

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
        }

        public void ChangeStateImmediately(string state)
        {
            
        }

    }
}