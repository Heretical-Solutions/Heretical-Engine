using System.Collections.Generic;
using System.Linq;

namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Class for tracking the progress of quests
    /// </summary>
    public class QuestProgressTracker
    {
        private readonly List<ActiveQuestObjective> activeObjectives;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestProgressTracker"/> class
        /// </summary>
        /// <param name="activeObjectives">The list of active quest objectives.</param>
        public QuestProgressTracker(List<ActiveQuestObjective> activeObjectives)
        {
            this.activeObjectives = activeObjectives;
        }

        /// <summary>
        /// Gets the number of active objectives
        /// </summary>
        public int ActiveObjectivesCount { get => activeObjectives.Count; }

        /// <summary>
        /// Gets all active quest objectives
        /// </summary>
        public IEnumerable<ActiveQuestObjective> AllActiveObjectives { get => activeObjectives; }

        /// <summary>
        /// Adds an objective to the list of active quest objectives
        /// </summary>
        /// <param name="tracker">The objective to add.</param>
        public void AddObjective(ActiveQuestObjective tracker)
        {
            activeObjectives.Add(tracker);
        }

        /// <summary>
        /// Gets the objective with the specified ID
        /// </summary>
        /// <param name="objectiveID">The ID of the objective.</param>
        /// <returns>The objective with the specified ID, or null if not found.</returns>
        public ActiveQuestObjective GetObjectiveByID(string objectiveID)
        {
            return activeObjectives.FirstOrDefault(item => item.Objective.Descriptor.ObjectiveID == objectiveID);
        }

        /// <summary>
        /// Gets the objective at the specified index
        /// </summary>
        /// <param name="index">The index of the objective.</param>
        /// <returns>The objective at the specified index, or null if not found.</returns>
        public ActiveQuestObjective GetObjectiveByIndex(int index)
        {
            return activeObjectives.FirstOrDefault(item => item.Objective.Index == index);
        }

        /// <summary>
        /// Removes an objective from the list of active quest objectives
        /// </summary>
        /// <param name="tracker">The objective to remove.</param>
        public void RemoveObjective(ActiveQuestObjective tracker)
        {
            activeObjectives.Remove(tracker);
        }

        /// <summary>
        /// Gets a value indicating whether all active objectives have been validated
        /// </summary>
        public bool AllActiveObjectivesValidated
        {
            get
            {
                if (ActiveObjectivesCount == 0)
                    return true;

                foreach (var activeObjective in activeObjectives)
                {
                    if (!activeObjective.Validate())
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Cleans up the progress of all active objectives and clears the list of active objectives
        /// </summary>
        public void Cleanup()
        {
            foreach (var tracker in activeObjectives)
            {
                tracker.Progress.Cleanup();
            }

            activeObjectives.Clear();
        }
    }
}