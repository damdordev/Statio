using System;
using System.Collections.Generic;
using Damdor.Vario;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Damdor.Statio
{
    [Serializable]
    public class StatioParameter {}
    
    [Serializable] 
    public abstract class StatioParameter<TComponent, TValue> : StatioParameter, IStatioParameterLifecycle, IStatioParameter<TComponent, TValue>
        where TComponent : Object
    {
        Vario.VarioStorage IStatioParameterLifecycle.Storage
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

        void IStatioParameterLifecycle.SaveSnapshot()
        {
            var component = ResolveTarget();
            if (component != null)
            {
                snapshot = GetValue(component);
            }
        }

        void IStatioParameterLifecycle.LoadDefaultValue()
        {
            var component = ResolveTarget();
            if (component != null)
            {
                var val = ResolveValue(defaultValue);
                SetValue(component, val);
            }
        }
        
        void IStatioParameterLifecycle.LoadValue(int stateId)
        {
            var component = ResolveTarget();
            if (component != null)
            {
                var val = ResolveValue(GetValue(stateId));
                SetValue(component, val);
            }
        }

        void IStatioParameterLifecycle.LoadValue(int stateId, float percentFromSnapshot)
        {
            var component = ResolveTarget();
            if (component != null)
            {
                var targetVal = ResolveValue(GetValue(stateId));
                SetValue(component, Lerp(snapshot, targetVal, percentFromSnapshot));
            }
        }

        void IStatioParameterLifecycle.SaveCurrentValueToDefaultValue()
        {
            defaultValue = new VarioValue<TValue>
            {
                Source = ValueSource.Raw,
                Value = GetValue(ResolveTarget())
            };
        }

        void IStatioParameterLifecycle.SaveCurrentValueToState(int stateId)
        {
            SetValue(stateId, new VarioValue<TValue>
            {
                Source = ValueSource.Raw,
                Value = GetValue(ResolveTarget())
            });
        }
        
        void IStatioParameterLifecycle.NotifyStateRemoved(int stateId)
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
        
        void IStatioParameterLifecycle.NotifyStateChanged(int oldIndex, int newIndex)
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
        {
            var numericOperations = VarioSettings.GetNumericOperations<TValue>();
            if (numericOperations == null) return t < 0.5f ? a : b;

            return numericOperations.LerpUnclamped(a, b, t);
        }

        private TComponent ResolveTarget()
        {
            return target.Evaluate(storage); 
        }

        protected T ResolveValue<T>(VarioValue<T> storageValue)
        {
            return storageValue.Evaluate(storage);
        }
        
    }
    
    [Serializable]
    public struct VisualStateParameterValue<TValue>
    {
        public int stateId;
        public VarioValue<TValue> value;
    }
    
}