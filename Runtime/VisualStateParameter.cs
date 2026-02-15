using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Damdor.VisualStates
{
    [Serializable]
    public abstract class VisualStateParameter
    {
        public abstract void LoadValue(int stateId, float percentFromSnapshot);
        public abstract void LoadDefaultValue();
        public abstract void MakeSnapshot();
        
        public abstract void NotifyStateChanged(int oldIndex, int newIndex);
    }

    [Serializable]
    public abstract class VisualStateParameter<TComponent, TValue> : VisualStateParameter
        where TComponent : Object
    {
        [SerializeField] private TComponent target;
        [SerializeField] private TValue defaultValue;
        [SerializeField] private List<VisualStateParameterValue<TValue>> values;

        private TValue snapshot;

        public override void MakeSnapshot()
        {
            snapshot = GetValue(target);
        }

        public override void LoadDefaultValue()
        {
            SetValue(target, defaultValue);
        }

        public override void LoadValue(int stateId, float percentFromSnapshot)
        {
            SetValue(target, Lerp(snapshot, GetValueForState(stateId), percentFromSnapshot));
        }

        public override void NotifyStateChanged(int oldIndex, int newIndex)
        {
        }

        protected abstract TValue GetValue(TComponent target);
        protected abstract void SetValue(TComponent target, TValue value);
        protected abstract TValue Lerp(TValue a, TValue b, float t);

        private TValue GetValueForState(int stateId)
        {
            foreach (var value in values)
            {
                if (value.stateId == stateId) return value.value;
            }

            return defaultValue;
        }

    }

    [Serializable]
    public struct VisualStateParameterValue<TValue>
    {
        public int stateId;
        public TValue value;
    }
}