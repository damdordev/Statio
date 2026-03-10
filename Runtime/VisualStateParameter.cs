using System;
using System.Collections.Generic;
using Damdor.Foundation;
using Damdor.Vario;
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
        Vario.VarioStorage IVisualStateParameterLifecycle.Storage
        {
            get => storage;
            set => storage = value;
        }
        
        public VarioValue<TComponent> Target
        {
            get => target;
            set => target = value;
        }

        public VarioValue<TValue> DefaultValue
        {
            get => defaultValue;
            set => defaultValue = value;
        }

        [SerializeField] private VarioValue<TComponent> target;
        [SerializeField] private VarioValue<TValue> defaultValue;
        [SerializeField] private List<VisualStateParameterValue<TValue>> values = new();

        private TValue snapshot;
        private Vario.VarioStorage storage;

        public void SetValue(int stateId, VarioValue<TValue> value)
        {
            for (var i = 0; i < values.Count; i++)
            {
                if (values[i].stateId != stateId) continue;
                values[i] = new VisualStateParameterValue<TValue> { stateId = stateId, value = value };
                return;
            }
            values.Add(new VisualStateParameterValue<TValue> { stateId = stateId, value = value });
        }

        public VarioValue<TValue> GetValue(int stateId)
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

        void IVisualStateParameterLifecycle.SaveCurrentValueToDefaultValue()
        {
            defaultValue = new VarioValue<TValue>
            {
                Source = ValueSource.Raw,
                Value = GetValue(ResolveTarget())
            };
        }

        void IVisualStateParameterLifecycle.SaveCurrentValueToState(int stateId)
        {
            SetValue(stateId, new VarioValue<TValue>
            {
                Source = ValueSource.Raw,
                Value = GetValue(ResolveTarget())
            });
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

        protected virtual TValue Lerp(TValue a, TValue b, float t)
            => NumericOperations.Get<TValue>().LerpUnclamped(a, b, t);

        private TComponent ResolveTarget()
        {
            if (storage == null) return target.Value;
            return storage.Evaluate(target); 
        }

        protected T ResolveValue<T>(VarioValue<T> storageValue)
        {
            if (storage == null) return storageValue.Value;
            return storage.Evaluate(storageValue);
        }
        
        protected T ResolveValue<T, TSerialized>(VarioValue<T, TSerialized> storageValue)
        {
            if (storage == null) return storageValue.Value;
            return storage.Evaluate(storageValue);
        }
    }
    
    [Serializable] 
    public abstract class VisualStateParameter<TComponent, TSerializedValue, TValue> : VisualStateParameter, IVisualStateParameterLifecycle, IVisualStateParameter<TComponent, TValue, TSerializedValue>
        where TComponent : Object
    {
        Vario.VarioStorage IVisualStateParameterLifecycle.Storage
        {
            get => storage;
            set => storage = value;
        }
        
        public VarioValue<TComponent> Target
        {
            get => target;
            set => target = value;
        }

        public VarioValue<TValue, TSerializedValue> DefaultValue
        {
            get => defaultValue;
            set => defaultValue = value;
        }

        [SerializeField] private VarioValue<TComponent> target;
        [SerializeField] private VarioValue<TValue, TSerializedValue> defaultValue;
        [SerializeField] private List<VisualStateParameterValue<TValue, TSerializedValue>> values = new();

        private TValue snapshot;
        private Vario.VarioStorage storage;

        public void SetValue(int stateId, VarioValue<TValue, TSerializedValue> value)
        {
            for (var i = 0; i < values.Count; i++)
            {
                if (values[i].stateId != stateId) continue;
                values[i] = new VisualStateParameterValue<TValue, TSerializedValue> { stateId = stateId, value = value };
                return;
            }
            values.Add(new VisualStateParameterValue<TValue, TSerializedValue> { stateId = stateId, value = value });
        }

        public VarioValue<TValue, TSerializedValue> GetValue(int stateId)
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
        
        void IVisualStateParameterLifecycle.SaveCurrentValueToDefaultValue()
        {
            defaultValue = new VarioValue<TValue, TSerializedValue>
            {
                Source = ValueSource.Raw,
                Value = GetValue(ResolveTarget())
            };
        }

        void IVisualStateParameterLifecycle.SaveCurrentValueToState(int stateId)
        {
            SetValue(stateId, new VarioValue<TValue, TSerializedValue>
            {
                Source = ValueSource.Raw,
                Value = GetValue(ResolveTarget())
            });
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

        protected virtual TValue Lerp(TValue a, TValue b, float t)
            => NumericOperations.Get<TValue>().LerpUnclamped(a, b, t);
        private TComponent ResolveTarget()
        {
            if (storage == null) return target.Value;
            return storage.Evaluate(target); 
        }

        protected T ResolveValue<T>(VarioValue<T> storageValue)
        {
            if (storage == null) return storageValue.Value;
            return storage.Evaluate(storageValue);
        }
        
        protected T ResolveValue<T, TSerialized>(VarioValue<T, TSerialized> storageValue)
        {
            if (storage == null) return storageValue.Value;
            return storage.Evaluate(storageValue);
        }
    }

    [Serializable]
    public struct VisualStateParameterValue<TValue>
    {
        public int stateId;
        public VarioValue<TValue> value;
    }
    
    [Serializable]
    public struct VisualStateParameterValue<TValue, TSerializableValue>
    {
        public int stateId;
        public VarioValue<TValue, TSerializableValue> value;
    }
}