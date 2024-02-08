namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Represents the types of stages that a quest can have
    /// </summary>
    public enum EStageTypes
    {
        /// <summary>
        /// Represents a checkpoint stage
        /// </summary>
        CHECKPOINT,
        
        /// <summary>
        /// Represents an action stage
        /// </summary>
        ACTION,
        
        /// <summary>
        /// Represents a tracker stage
        /// </summary>
        TRACKER,
        
        /// <summary>
        /// Represents an awaiter stage
        /// </summary>
        AWAITER
    }
}