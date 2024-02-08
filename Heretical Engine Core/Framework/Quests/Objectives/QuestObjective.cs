namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Represents an objective of a quest
    /// </summary>
    public class QuestObjective
    {
        /// <summary>
        /// Gets the index of the objective
        /// </summary>
        public int Index { get; private set; }
        
        /// <summary>
        /// Gets the descriptor of the objective
        /// </summary>
        public QuestObjectiveDescriptor Descriptor { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the objective is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the current value of the objective
        /// </summary>
        public int CurrentValue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestObjective"/> class
        /// </summary>
        /// <param name="index">The index of the objective.</param>
        /// <param name="descriptor">The descriptor of the objective.</param>
        /// <param name="active">A value indicating whether the objective is active.</param>
        /// <param name="currentValue">The current value of the objective.</param>
        public QuestObjective(
            int index,
            QuestObjectiveDescriptor descriptor,
            bool active,
            int currentValue)
        {
            Index = index;
            
            Descriptor = descriptor;

            Active = active;

            CurrentValue = currentValue;
        }
    }
}