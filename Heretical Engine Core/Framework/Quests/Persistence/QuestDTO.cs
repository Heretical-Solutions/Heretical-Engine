namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Represents a data transfer object for a quest
    /// </summary>
    [System.Serializable]
    public struct QuestDTO
    {
        /// <summary>
        /// The unique identifier of the quest
        /// </summary>
        public string QuestID;

        /// <summary>
        /// The current status of the quest
        /// </summary>
        public EQuestStatus Status;

        /// <summary>
        /// An array of quest objective data trackers
        /// </summary>
        public QuestObjectiveDTO[] ObjectiveTrackersData;
    }
}