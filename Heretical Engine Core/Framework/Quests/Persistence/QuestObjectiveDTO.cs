namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Represents a data transfer object for a quest objective
    /// </summary>
    public struct QuestObjectiveDTO
    {
        /// <summary>
        /// Gets or sets the index of the objective
        /// </summary>
        public int ObjectiveIndex;

        /// <summary>
        /// Gets or sets the ID of the objective
        /// </summary>
        public string ObjectiveID;

        /// <summary>
        /// Gets or sets if the objective is active
        /// </summary>
        public bool Active;

        /// <summary>
        /// Gets or sets the current value of the objective
        /// </summary>
        public int CurrentValue;
    }
}