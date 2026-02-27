namespace Damdor.VisualStates
{
    public static class VisualStateHelper
    {
        public static int RecalculateStateIdAfterStateRemoved(int removedStateId, int stateId)
        {
            if (stateId < 0) return stateId;
            if(stateId == removedStateId) return -1;
            if (stateId > removedStateId) return stateId - 1;
            return stateId;
        }
        
        public static int RecalculateStateIdAfterStateIdChanged(int oldMovedStateId, int newMovedStateId, int stateId)
        {
            if (stateId < 0 || oldMovedStateId == newMovedStateId) return stateId;
            if (stateId == oldMovedStateId) return newMovedStateId;

            if (oldMovedStateId < newMovedStateId && stateId > oldMovedStateId && stateId <= newMovedStateId)
            {
                return stateId - 1;
            }

            if (oldMovedStateId > newMovedStateId && stateId >= newMovedStateId && stateId < oldMovedStateId)
            {
                return stateId + 1;
            }

            return stateId;
        }
    }
}