using System;
using System.Collections.Generic;
using Damdor.VariableStorage;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Damdor.VisualStates
{
    [Serializable] 
    public abstract class VisualStateParameter<TComponent, TValue> : IVisualStateParameterLifecycle, IVisualStateParameter<TComponent, TValue>
        where TComponent : Object
    {
        [SerializeField] private StorageValue<TComponent> target;
        [SerializeField] private StorageValue<TValue> defaultValue;
        [SerializeField] private List<VisualStateParameterValue<TValue>> values;

        private TValue snapshot;
    }

    [Serializable]
    public struct VisualStateParameterValue<TValue>
    {
        public int stateId;
        public StorageValue<TValue> value;
    }
}