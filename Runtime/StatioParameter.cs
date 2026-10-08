using System;
using System.Collections.Generic;
using Damdor.Vario;
using UnityEngine;
using Object = UnityEngine.Object;
// ReSharper disable Unity.PerformanceCriticalCodeNullComparison

namespace Damdor.Statio
{
    /// <summary>
    /// Provides a common serializable base for Statio parameters.
    /// </summary>
    [Serializable]
    public class StatioParameter {}
    
    /// <summary>
    /// Controls a Unity object property using a default value, state overrides, and transition snapshots.
    /// </summary>
    /// <typeparam name="TComponent">The Unity object type to control.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <remarks>Targets and values are evaluated using the storage assigned through IStatioParameterLifecycle. Missing targets are skipped when loading values or saving snapshots.</remarks>
    [Serializable] 
    public abstract class StatioParameter<TComponent, TValue> : StatioParameter, IStatioParameterLifecycle, IStatioParameter<TComponent, TValue>
        where TComponent : Object
    {
        VarioStorage IStatioParameterLifecycle.Storage
        {
            get => storage;
            set => storage = value;
        }
        
        /// <inheritdoc/>
        public VarioValue<TComponent> Target
        {
            get => target;
            set => target = value;
        }

        /// <inheritdoc/>
        public VarioValue<TValue> DefaultValue
        {
            get => defaultValue;
            set => defaultValue = value;
        }

        [SerializeField] private VarioValue<TComponent> target;
        [SerializeField] private VarioValue<TValue> defaultValue;
        [SerializeField] private List<VisualStateParameterValue<TValue>> values = new();

        private TValue snapshot;
        private VarioStorage storage;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public VarioValue<TValue> GetValue(int stateId)
        {
            foreach (var value in values)
            {
                if (value.stateId == stateId) return value.value;
            }
            return defaultValue;
        }

        /// <inheritdoc/>
        public bool HasOverride(int stateId)
        {
            foreach (var value in values)
            {
                if (value.stateId == stateId) return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public void RemoveOverride(int stateId)
        {
            values.RemoveAll(v => v.stateId == stateId);
        }

        void IStatioParameterLifecycle.SaveSnapshot()
        {
            var component = ResolveTarget();
            if (component == null) return;
            snapshot = GetValue(component);
        }

        void IStatioParameterLifecycle.LoadDefaultValue()
        {
            var component = ResolveTarget();
            if (component == null) return;
            var val = ResolveValue(defaultValue);
            SetValue(component, val);
        }
        
        void IStatioParameterLifecycle.LoadValue(int stateId)
        {
            var component = ResolveTarget();
            if (component == null) return;
            var val = ResolveValue(GetValue(stateId));
            SetValue(component, val);
        }

        void IStatioParameterLifecycle.LoadValue(int stateId, float percentFromSnapshot)
        {
            var component = ResolveTarget();
            if (component == null) return;
            var targetVal = ResolveValue(GetValue(stateId));
            SetValue(component, Lerp(snapshot, targetVal, percentFromSnapshot));
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
                    values[index] = new VisualStateParameterValue<TValue>
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

        /// <summary>
        /// Reads the controlled property from the resolved target.
        /// </summary>
        /// <param name="target">The resolved Unity object.</param>
        /// <returns>The current property value.</returns>
        protected abstract TValue GetValue(TComponent target);
        /// <summary>
        /// Writes the controlled property to the resolved target.
        /// </summary>
        /// <param name="target">The resolved Unity object.</param>
        /// <param name="value">The property value to apply.</param>
        protected abstract void SetValue(TComponent target, TValue value);

        /// <summary>
        /// Interpolates through Vario numeric operations, or switches at 0.5 when none are registered.
        /// </summary>
        /// <param name="a">The saved snapshot value.</param>
        /// <param name="b">The destination value.</param>
        /// <param name="t">The eased interpolation factor, which may be outside the range zero to one.</param>
        /// <returns>The interpolated or selected value.</returns>
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

        /// <summary>
        /// Evaluates a Vario value using this parameter’s assigned storage.
        /// </summary>
        /// <typeparam name="T">The evaluated value type.</typeparam>
        /// <param name="storageValue">The value expression to evaluate.</param>
        /// <returns>The evaluated value.</returns>
        protected T ResolveValue<T>(VarioValue<T> storageValue)
        {
            return storageValue.Evaluate(storage);
        }
        
    }
    
    /// <summary>
    /// Stores a value override associated with a state index.
    /// </summary>
    /// <typeparam name="TValue">The override value type.</typeparam>
    [Serializable]
    public struct VisualStateParameterValue<TValue>
    {
        /// <summary>
        /// The zero-based state index.
        /// </summary>
        public int stateId;
      
        /// <summary>
        /// The value expression for this state.
        /// </summary>
        public VarioValue<TValue> value;
    }
    
}