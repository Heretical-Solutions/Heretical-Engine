using System;

namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Represents an active quest in the game
    /// </summary>
    public class ActiveQuest
    {
        /// <summary>
        /// Gets the quest associated with this active quest
        /// </summary>
        public Quest Quest { get; private set; }

        #region Current stage

        /// <summary>
        /// Gets or sets the current stage of the quest
        /// </summary>
        public QuestStage CurrentStage { get; set; }

        private int currentStageIndex;

        /// <summary>
        /// Gets the index of the current stage
        /// </summary>
        public int CurrentStageIndex { get =>  currentStageIndex; }

        //public QuestStageDTO CurrentStageData { get => Quest.Descriptor.Stages[currentStageIndex]; }

        #endregion

        #region Progress
        
        /// <summary>
        /// Gets the progress tracker for the quest
        /// </summary>
        public QuestProgressTracker QuestProgressTracker { get; private set; }

        #endregion
        
        #region Callbacks
        
        /// <summary>
        /// Gets or sets the callback for when the quest is started
        /// </summary>
        public Action<ActiveQuest> OnQuestStarted { get; set; }
        
        /// <summary>
        /// Gets or sets the callback for when a stage of the quest is started
        /// </summary>
        public Action<ActiveQuest> OnStageStarted { get; set; }
        
        /// <summary>
        /// Gets or sets the callback for when a stage of the quest is completed
        /// </summary>
        public Action<ActiveQuest> OnStageCompleted { get; set; }
        
        /// <summary>
        /// Gets or sets the callback for when the quest is completed
        /// </summary>
        public Action<ActiveQuest> OnQuestCompleted { get; set; }

        #endregion
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveQuest"/> class
        /// </summary>
        /// <param name="quest">The quest associated with this active quest.</param>
        /// <param name="questProgressTracker">The progress tracker for the quest.</param>
        public ActiveQuest(
            Quest quest,
            QuestProgressTracker questProgressTracker)
        {
            Quest = quest;

            currentStageIndex = 0;

            QuestProgressTracker = questProgressTracker;
        }

        /// <summary>
        /// Starts the quest
        /// </summary>
        public void Start()
        {
            Quest.Status = EQuestStatus.ACTIVE;
            
            OnQuestStarted?.Invoke(this);
            
            if (currentStageIndex >= Quest.Descriptor.Stages.Length)
                QuestComplete();
            else
            {
                StartNextStage();
            }
        }

        /// <summary>
        /// Marks the current stage as complete and proceeds to the next stage of the quest
        /// </summary>
        public void StageComplete()
        {
            OnStageCompleted?.Invoke(this);

            currentStageIndex++;

            if (currentStageIndex >= Quest.Descriptor.Stages.Length)
                QuestComplete();
            else
            {
                StartNextStage();
            }
        }

        private void StartNextStage()
        {
            OnStageStarted?.Invoke(this);
        }

        private void QuestComplete()
        {
            Quest.Status = EQuestStatus.COMPLETED;

            OnQuestCompleted?.Invoke(this);

            QuestProgressTracker.Cleanup();
        }
    }
}