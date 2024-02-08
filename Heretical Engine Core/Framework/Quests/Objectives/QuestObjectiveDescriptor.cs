namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Represents a descriptor for a quest objective
    /// </summary>
    [System.Serializable]
    public struct QuestObjectiveDescriptor
    {
        /// <summary>
        /// The ID of the objective
        /// </summary>
        public string ObjectiveID;

        /// <summary>
        /// The description of the objective
        /// </summary>
        public string Description;

        /// <summary>
        /// The comparison type to be used for tracking the objective
        /// </summary>
        public ETrackerComparison Comparison;

        /// <summary>
        /// The expected value for completing the objective
        /// </summary>
        public int ExpectedValue;

        /// <summary>
        /// The properties associated with the objective
        /// </summary>
        public QuestPropertyDTO[] Properties;
    }
}