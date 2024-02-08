using System;

namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Represents a stage in a quest
    /// </summary>
    public class QuestStage
    {
        /// <summary>
        /// Gets the descriptor for this quest stage
        /// </summary>
        public QuestStageDescriptor Descriptor { get; private set; }

        /// <summary>
        /// Gets the properties associated with this quest stage
        /// </summary>
        public QuestPropertyDTO[] Properties { get; private set; }
        
        /// <summary>
        /// Gets or sets the handler for this quest stage
        /// </summary>
        public Action<ActiveQuestsManager, ActiveQuest> Handler { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestStage"/> class
        /// </summary>
        /// <param name="descriptor">The descriptor for the quest stage.</param>
        /// <param name="handler">The handler for the quest stage.</param>
        public QuestStage(
            QuestStageDescriptor descriptor,
            Action<ActiveQuestsManager, ActiveQuest> handler)
        {
            Descriptor = descriptor;

            Properties = Array.Empty<QuestPropertyDTO>();

            Handler = handler;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestStage"/> class with the specified prototype and properties
        /// </summary>
        /// <param name="prototype">The prototype to create the quest stage from.</param>
        /// <param name="properties">The properties for the quest stage.</param>
        public QuestStage(
            QuestStage prototype,
            QuestPropertyDTO[] properties)
        {
            Descriptor = prototype.Descriptor;

            Properties = properties;

            Handler = prototype.Handler;
        }
    }
}