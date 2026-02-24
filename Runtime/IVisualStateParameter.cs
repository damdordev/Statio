using Damdor.VariableStorage;
using UnityEngine;

namespace Damdor.VisualStates
{
    public interface IVisualStateParameter<TComponent, TValue>
        where TComponent : Object
    {
        StorageValue<TComponent> Target { get; set; }
        StorageValue<TValue> DefaultValue { get; set; }
        
        void SetValue(int stateId, StorageValue<TValue> value);
        StorageValue<TValue> GetValue(int stateId);
        bool HasOverride(int stateId);
        void RemoveOverride(int stateId);
    }
    
    public interface IVisualStateParameter<TComponent, TSerializedValue, TValue>
        where TComponent : Object
    {
        StorageValue<TComponent> Target { get; set; }
        StorageValue<TValue, TSerializedValue> DefaultValue { get; set; }
        
        void SetValue(int stateId, StorageValue<TValue, TSerializedValue> value);
        StorageValue<TSerializedValue, TValue> GetValue(int stateId);
        bool HasOverride(int stateId);
        void RemoveOverride(int stateId);
    }
}