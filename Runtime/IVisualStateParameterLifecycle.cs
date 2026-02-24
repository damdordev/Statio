namespace Damdor.VisualStates
{
    public interface IVisualStateParameterLifecycle
    {
        void LoadDefaultValue();
        void LoadValue(int stateId);
        void LoadValue(int stateId, float percentFromSnapshot);
        void SaveSnapshot();
        void NotifyStateRemoved(int stateId);
        void NotifyStateChanged(int oldIndex, int newIndex);
    }
}