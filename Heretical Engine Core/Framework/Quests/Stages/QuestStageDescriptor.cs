using System;

namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Represents a quest stage descriptor
    /// </summary>
    [System.Serializable]
    public struct QuestStageDescriptor
    {
        /// <summary>
        /// The unique identifier for the quest action
        /// </summary>
        public string ActionID;

        /// <summary>
        /// The stage type of the quest stage descriptor
        /// </summary>
        public EStageTypes StageType;
    }
}