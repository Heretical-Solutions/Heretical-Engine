using System.Collections.Generic;
using HereticalSolutions.MVVM.Observable;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Quests.Factories
{
    /// <summary>
    /// A factory class for building quests and related objects
    /// </summary>
    public static partial class QuestsFactory
    {
        /// <summary>
        /// Builds a quest with the specified descriptor and status
        /// </summary>
        /// <param name="questDescriptor">The descriptor of the quest.</param>
        /// <param name="status">The status of the quest.</param>
        /// <returns>The built quest object.</returns>
        public static Quest BuildQuest(
            QuestDescriptor questDescriptor,
            EQuestStatus status)
        {
            var objectives = new QuestObjective[questDescriptor.Objectives.Length];

            for (int i = 0; i < objectives.Length; i++)
                objectives[i] = BuildQuestObjective(
                    i,
                    questDescriptor.Objectives[i],
                    false,
                    0);

            return new Quest(
                questDescriptor,
                status,
                objectives);
        }

        /// <summary>
        /// Builds a quest objective with the specified index, descriptor, active status, and current value
        /// </summary>
        /// <param name="index">The index of the quest objective.</param>
        /// <param name="descriptor">The descriptor of the quest objective.</param>
        /// <param name="active">The active status of the quest objective.</param>
        /// <param name="currentValue">The current value of the quest objective.</param>
        /// <returns>The built quest objective object.</returns>
        private static QuestObjective BuildQuestObjective(
            int index,
            QuestObjectiveDescriptor descriptor,
            bool active,
            int currentValue)
        {
            return new QuestObjective(
                index,
                descriptor,
                active,
                currentValue);
        }

        /// <summary>
        /// Builds an active quest object with the specified quest
        /// </summary>
        /// <param name="quest">The quest associated with the active quest.</param>
        /// <returns>The built active quest object.</returns>
        public static ActiveQuest BuildActiveQuest(Quest quest)
        {
            return new ActiveQuest(
                quest,
                new QuestProgressTracker(
                    new List<ActiveQuestObjective>()));
        }

        /// <summary>
        /// Builds an active quest objectives manager object
        /// </summary>
        /// <returns>The built active quest objectives manager object.</returns>
        public static ActiveQuestObjectivesManager BuildActiveQuestObjectivesManager()
        {
            return new ActiveQuestObjectivesManager(
                RepositoriesFactory.BuildDictionaryRepository<string, List<ActiveQuestObjective>>());
        }

        /// <summary>
        /// Builds an active quest objective with the specified quest objective
        /// </summary>
        /// <param name="questObjective">The quest objective associated with the active quest objective.</param>
        /// <returns>The built active quest objective object.</returns>
        public static ActiveQuestObjective BuildActiveQuestObjective(
            QuestObjective questObjective)
        {
            ObservableProperty<int> observable = new ObservableProperty<int>(questObjective.CurrentValue);

            return new ActiveQuestObjective(
                questObjective,
                observable);
        }
    }
}