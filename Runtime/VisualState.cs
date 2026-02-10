using UnityEngine;
using Damdor.VariableStorage;

namespace Damdor.VisualStates
{
    public class VisualState : MonoBehaviour, IVariableStorageSource
    {
        public VariableStorage.VariableStorage Storage => storage;

        [SerializeField] private VariableStorage.VariableStorage storage;
    }
}