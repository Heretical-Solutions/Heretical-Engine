namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Represents a data transfer object for a quest stage
    /// </summary>
    [System.Serializable]
    public struct QuestStageDTO
    {
        /// <summary>
        /// The ID of the action associated with the quest stage
        /// </summary>
        public string ActionID;

        /// <summary>
        /// An array of quest properties associated with the quest stage
        /// </summary>
        public QuestPropertyDTO[] Properties;
    }
}