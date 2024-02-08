using System.Collections.Generic;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Manages the active quest objectives
    /// </summary>
    public class ActiveQuestObjectivesManager
    {
        private readonly IRepository<string, List<ActiveQuestObjective>> activeObjectivesRepository;

        /// <summary>
        /// Initializes a new instance of the ActiveQuestObjectivesManager class with the specified active objectives repository
        /// </summary>
        /// <param name="activeObjectivesRepository">The repository for storing active quest objectives.</param>
        public ActiveQuestObjectivesManager(IRepository<string, List<ActiveQuestObjective>> activeObjectivesRepository)
        {
            this.activeObjectivesRepository = activeObjectivesRepository;
        }

        /// <summary>
        /// Adds an active quest objective
        /// </summary>
        /// <param name="activeObjective">The active quest objective to add.</param>
        public void AddObjective(ActiveQuestObjective activeObjective)
        {
            string objectiveID = activeObjective.Objective.Descriptor.ObjectiveID;
            
            // Add a new list of active objectives to the repository if the objective ID doesn't exist
            if (!activeObjectivesRepository.Has(objectiveID))
            {
                activeObjectivesRepository.Add(objectiveID, new List<ActiveQuestObjective>());
            }
            
            // Retrieve the list of active objectives by ID and add the new active objective to it
            var trackersByID = activeObjectivesRepository.Get(objectiveID);
            trackersByID.Add(activeObjective);
        }

        /// <summary>
        /// Gets the active quest objectives with the specified objective ID
        /// </summary>
        /// <param name="objectiveID">The ID of the objective.</param>
        /// <param name="activeObjectives">When this method returns, contains the active quest objectives with the specified objective ID, if found; otherwise, an empty collection.</param>
        /// <returns>true if the active quest objectives with the specified objective ID were found; otherwise, false.</returns>
        public bool GetObjectives(string objectiveID, out IEnumerable<ActiveQuestObjective> activeObjectives)
        {
            var result = activeObjectivesRepository.TryGet(objectiveID, out List<ActiveQuestObjective> objectivesList);

            activeObjectives = objectivesList;

            return result;
        }

        /// <summary>
        /// Gets all active quest objectives
        /// </summary>
        /// <returns>A collection of all active quest objectives.</returns>
        public IEnumerable<ActiveQuestObjective> GetAllObjectives()
        {
            List<ActiveQuestObjective> result = new List<ActiveQuestObjective>();

            var keys = activeObjectivesRepository.Keys;
            
            foreach (var key in keys)
                result.AddRange(activeObjectivesRepository.Get(key));

            return result;
        }

        /// <summary>
        /// Removes an active quest objective
        /// </summary>
        /// <param name="activeObjective">The active quest objective to remove.</param>
        public void RemoveObjective(ActiveQuestObjective activeObjective)
        {
            // Check if the objective ID exists in the repository and remove the active objective from it
            if (!activeObjectivesRepository.Has(activeObjective.Objective.Descriptor.ObjectiveID))
                return;
                
            activeObjectivesRepository.Get(activeObjective.Objective.Descriptor.ObjectiveID).Remove(activeObjective);
        }
    }
}