namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Struct that represents a quest property with a key and value
    /// </summary>
    [System.Serializable]
    public struct QuestPropertyDTO
    {
        /// <summary>
        /// The key of the quest property
        /// </summary>
        public string Key;

        /// <summary>
        /// The value of the quest property
        /// </summary>
        public string Value;
    }
}