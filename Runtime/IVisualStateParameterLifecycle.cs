namespace Damdor.VisualStates
{
    public interface IVisualStateParameterLifecycle
    {
        VariableStorage.VariableStorage Storage { get; set; }
        VariableStorage.VariableStorage ParentStorage { get; set; }
        void LoadDefaultValue();
        void LoadValue(int stateId);
        void LoadValue(int stateId, float percentFromSnapshot);
        void SaveSnapshot();
        void NotifyStateRemoved(int stateId);
        void NotifyStateChanged(int oldIndex, int newIndex);
    }
}