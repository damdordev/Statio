using System;
using System.Collections.Generic;
using Damdor.VariableStorage;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Damdor.VisualStates
{
    [Serializable]
    public class VisualStateParameter {}
    
    [Serializable] 
    public abstract class VisualStateParameter<TComponent, TValue> : VisualStateParameter, IVisualStateParameterLifecycle, IVisualStateParameter<TComponent, TValue>
        where TComponent : Object
    {
        VariableStorage.VariableStorage IVisualStateParameterLifecycle.Storage
        {
            get => storage;
            set => storage = value;
        }
        
        VariableStorage.VariableStorage IVisualStateParameterLifecycle.ParentStorage
        {
            get => parentStorage;
            set => parentStorage = value;
        }
        
        public StorageValue<TComponent> Target
        {
            get => target;
            set => target = value;
        }

        public StorageValue<TValue> DefaultValue
        {
            get => defaultValue;
            set => defaultValue = value;
        }

        [SerializeField] private StorageValue<TComponent> target;
        [SerializeField] private StorageValue<TValue> defaultValue;
        [SerializeField] private List<VisualStateParameterValue<TValue>> values = new();

        private TValue snapshot;
        private VariableStorage.VariableStorage storage;
        private VariableStorage.VariableStorage parentStorage;

        public void SetValue(int stateId, StorageValue<TValue> value)
        {
            for (var i = 0; i < values.Count; i++)
            {
                if (values[i].stateId != stateId) continue;
                values[i] = new VisualStateParameterValue<TValue> { stateId = stateId, value = value };
                return;
            }
            values.Add(new VisualStateParameterValue<TValue> { stateId = stateId, value = value });
        }

        public StorageValue<TValue> GetValue(int stateId)
        {
            foreach (var value in values)
            {
                if (value.stateId == stateId) return value.value;
            }
            return defaultValue;
        }

        public bool HasOverride(int stateId)
        {
            foreach (var value in values)
            {
                if (value.stateId == stateId) return true;
            }
            return false;
        }

        public void RemoveOverride(int stateId)
        {
            values.RemoveAll(v => v.stateId == stateId);
        }

        void IVisualStateParameterLifecycle.SaveSnapshot()
        {
            var component = ResolveTarget();
            if (component != null)
            {
                snapshot = GetValue(component);
            }
        }

        void IVisualStateParameterLifecycle.LoadDefaultValue()
        {
            var component = ResolveTarget();
            if (component != null)
            {
                var val = ResolveValue(defaultValue);
                SetValue(component, val);
            }
        }
        
        void IVisualStateParameterLifecycle.LoadValue(int stateId)
        {
            var component = ResolveTarget();
            if (component != null)
            {
                var val = ResolveValue(GetValue(stateId));
                SetValue(component, val);
            }
        }

        void IVisualStateParameterLifecycle.LoadValue(int stateId, float percentFromSnapshot)
        {
            var component = ResolveTarget();
            if (component != null)
            {
                var targetVal = ResolveValue(GetValue(stateId));
                SetValue(component, Lerp(snapshot, targetVal, percentFromSnapshot));
            }
        }

        void IVisualStateParameterLifecycle.NotifyStateRemoved(int stateId)
        {
            values.RemoveAll(v => v.stateId == stateId);
            for (var index = 0; index < values.Count; index++)
            {
                var visualStateParameterValue = values[index];
                if (visualStateParameterValue.stateId > stateId)
                {
                    values[index] = new VisualStateParameterValue<TValue>()
                    {
                        stateId = visualStateParameterValue.stateId - 1,
                        value = visualStateParameterValue.value
                    };
                }
            }
        }
        
        void IVisualStateParameterLifecycle.NotifyStateChanged(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex) return;

            if (oldIndex < newIndex)
            {
                for (var index = 0; index < values.Count; index++)
                {
                    var value = values[index];
                    if (value.stateId == oldIndex)
                    {
                        values[index] = new VisualStateParameterValue<TValue> { stateId = newIndex, value = value.value };
                    }
                    else if (value.stateId > oldIndex && value.stateId <= newIndex)
                    {
                        values[index] = new VisualStateParameterValue<TValue> { value = value.value, stateId = value.stateId - 1 };
                    }
                }
            }
            else
            {
                for (var index = 0; index < values.Count; index++)
                {
                    var value = values[index];
                    if (value.stateId == oldIndex)
                    {
                        values[index] = new VisualStateParameterValue<TValue> { stateId = newIndex, value = value.value };
                    }
                    else if (value.stateId >= newIndex && value.stateId < oldIndex)
                    {
                        values[index] = new VisualStateParameterValue<TValue> { value = value.value, stateId = value.stateId + 1 };
                    }
                }
            }
        }

        protected abstract TValue GetValue(TComponent target);
        protected abstract void SetValue(TComponent target, TValue value);
        protected abstract TValue Lerp(TValue a, TValue b, float t);

        private TComponent ResolveTarget()
        {
            if (storage == null) return target.Value;
            return storage.Evaluate(target, parentStorage); 
        }

        private TValue ResolveValue(StorageValue<TValue> storageValue)
        {
            if (storage == null) return storageValue.Value;
            return storage.Evaluate(storageValue, parentStorage);
        }
    }
    
    [Serializable] 
    public abstract class VisualStateParameter<TComponent, TSerializedValue, TValue> : VisualStateParameter, IVisualStateParameterLifecycle, IVisualStateParameter<TComponent, TValue, TSerializedValue>
        where TComponent : Object
    {
        VariableStorage.VariableStorage IVisualStateParameterLifecycle.Storage
        {
            get => storage;
            set => storage = value;
        }
        
        VariableStorage.VariableStorage IVisualStateParameterLifecycle.ParentStorage
        {
            get => parentStorage;
            set => parentStorage = value;
        }
        
        public StorageValue<TComponent> Target
        {
            get => target;
            set => target = value;
        }

        public StorageValue<TValue, TSerializedValue> DefaultValue
        {
            get => defaultValue;
            set => defaultValue = value;
        }

        [SerializeField] private StorageValue<TComponent> target;
        [SerializeField] private StorageValue<TValue, TSerializedValue> defaultValue;
        [SerializeField] private List<VisualStateParameterValue<TValue, TSerializedValue>> values = new();

        private TValue snapshot;
        private VariableStorage.VariableStorage storage;
        private VariableStorage.VariableStorage parentStorage;

        public void SetValue(int stateId, StorageValue<TValue, TSerializedValue> value)
        {
            for (var i = 0; i < values.Count; i++)
            {
                if (values[i].stateId != stateId) continue;
                values[i] = new VisualStateParameterValue<TValue, TSerializedValue> { stateId = stateId, value = value };
                return;
            }
            values.Add(new VisualStateParameterValue<TValue, TSerializedValue> { stateId = stateId, value = value });
        }

        public StorageValue<TValue, TSerializedValue> GetValue(int stateId)
        {
            foreach (var value in values)
            {
                if (value.stateId == stateId) return value.value;
            }
            return defaultValue;
        }

        public bool HasOverride(int stateId)
        {
            foreach (var value in values)
            {
                if (value.stateId == stateId) return true;
            }
            return false;
        }

        public void RemoveOverride(int stateId)
        {
            values.RemoveAll(v => v.stateId == stateId);
        }

        void IVisualStateParameterLifecycle.SaveSnapshot()
        {
            var component = ResolveTarget();
            if (component != null)
            {
                snapshot = GetValue(component);
            }
        }

        void IVisualStateParameterLifecycle.LoadDefaultValue()
        {
            var component = ResolveTarget();
            if (component != null)
            {
                var val = ResolveValue(defaultValue);
                SetValue(component, val);
            }
        }
        
        void IVisualStateParameterLifecycle.LoadValue(int stateId)
        {
            var component = ResolveTarget();
            if (component != null)
            {
                var val = ResolveValue(GetValue(stateId));
                SetValue(component, val);
            }
        }

        void IVisualStateParameterLifecycle.LoadValue(int stateId, float percentFromSnapshot)
        {
            var component = ResolveTarget();
            if (component != null)
            {
                var targetVal = ResolveValue(GetValue(stateId));
                SetValue(component, Lerp(snapshot, targetVal, percentFromSnapshot));
            }
        }

        void IVisualStateParameterLifecycle.NotifyStateRemoved(int removedStateId)
        {
            values.RemoveAll(v => v.stateId == removedStateId);
            for (var index = 0; index < values.Count; index++)
            {
                var visualStateParameterValue = values[index];
                var oldStateId = visualStateParameterValue.stateId;
                var newStateId = VisualStateHelper.RecalculateStateIdAfterStateRemoved(removedStateId, oldStateId);

                if (oldStateId == newStateId) continue;
                
                values[index] = new VisualStateParameterValue<TValue, TSerializedValue>()
                {
                    stateId = newStateId,
                    value = visualStateParameterValue.value
                };
            }
        }
        
        void IVisualStateParameterLifecycle.NotifyStateChanged(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex) return;

            for (var index = 0; index < values.Count; index++)
            {
                var value = values[index];
                var oldStateId = value.stateId;
                var newStateId = VisualStateHelper.RecalculateStateIdAfterStateIdChanged(oldIndex, newIndex, oldStateId);
                
                if(oldStateId == newStateId) continue;
                values[index] = new VisualStateParameterValue<TValue, TSerializedValue>
                {
                    value = value.value,
                    stateId = newStateId
                };
            }
        }

        protected abstract TValue GetValue(TComponent target);
        protected abstract void SetValue(TComponent target, TValue value);
        protected abstract TValue Lerp(TValue a, TValue b, float t);

        private TComponent ResolveTarget()
        {
            if (storage == null) return target.Value;
            return storage.Evaluate(target, parentStorage); 
        }

        private TValue ResolveValue(StorageValue<TValue, TSerializedValue> storageValue)
        {
            if (storage == null) return storageValue.Value;
            return storage.Evaluate(storageValue, parentStorage);
        }
    }

    [Serializable]
    public struct VisualStateParameterValue<TValue>
    {
        public int stateId;
        public StorageValue<TValue> value;
    }
    
    [Serializable]
    public struct VisualStateParameterValue<TValue, TSerializableValue>
    {
        public int stateId;
        public StorageValue<TValue, TSerializableValue> value;
    }
}