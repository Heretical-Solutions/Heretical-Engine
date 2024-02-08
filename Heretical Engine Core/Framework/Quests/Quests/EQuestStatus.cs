namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Enumeration representing the status of a quest
    /// </summary>
    [System.Serializable]
    public enum EQuestStatus
    {
        /// <summary>
        /// The quest is idle and not currently active
        /// </summary>
        IDLE,
        
        /// <summary>
        /// The quest is currently active
        /// </summary>
        ACTIVE,
        
        /// <summary>
        /// The quest has been completed
        /// </summary>
        COMPLETED,
        
        /// <summary>
        /// The quest has no status
        /// </summary>
        NONE
    }
}