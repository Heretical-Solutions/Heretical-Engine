using System;

namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Represents a quest descriptor
    /// </summary>
    [Serializable]
    public struct QuestDescriptor
    {
        /// <summary>
        /// The ID of the quest
        /// </summary>
        public string ID;
        
        /// <summary>
        /// The name of the quest
        /// </summary>
        public string Name;

        /// <summary>
        /// The description of the quest
        /// </summary>
        public string Description;

        /// <summary>
        /// The objectives of the quest
        /// </summary>
        public QuestObjectiveDescriptor[] Objectives;
        
        /// <summary>
        /// The stages of the quest
        /// </summary>
        public QuestStageDTO[] Stages;
    }
}
