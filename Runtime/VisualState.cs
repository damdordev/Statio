using System.Collections.Generic;
using UnityEngine;
using Damdor.VariableStorage;

namespace Damdor.VisualStates
{
    public class VisualState : MonoBehaviour, IVariableStorageSource
    {
        public VariableStorage.VariableStorage Storage => storage;
        public IReadOnlyList<string> States => states.States;

        [SerializeField] private VisualStatesList states;
        [SerializeField] private VariableStorage.VariableStorage storage;

        public void ChangeState(string state)
        {
        }

        public void ChangeStateImmediately(string state)
        {
            
        }

    }
}