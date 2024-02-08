namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Represents a quest
    /// </summary>
    public class Quest
    {
        /// <summary>
        /// Gets or sets the quest descriptor
        /// </summary>
        /// <value>The quest descriptor.</value>
        public QuestDescriptor Descriptor { get; private set; }
        
        /// <summary>
        /// Gets or sets the quest status
        /// </summary>
        /// <value>The quest status.</value>
        public EQuestStatus Status { get; set; }
        
        /// <summary>
        /// Gets or sets the quest objectives
        /// </summary>
        /// <value>The quest objectives.</value>
        public QuestObjective[] Objectives { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quest"/> class with the specified descriptor, status, and objectives
        /// </summary>
        /// <param name="descriptor">The quest descriptor.</param>
        /// <param name="status">The quest status.</param>
        /// <param name="objectives">The quest objectives.</param>
        public Quest(
            QuestDescriptor descriptor,
            EQuestStatus status,
            QuestObjective[] objectives)
        {
            Descriptor = descriptor;
            Status = status;
            Objectives = objectives;
        }
    }
}