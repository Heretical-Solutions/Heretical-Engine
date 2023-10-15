using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Quests.Factories
{
    public static partial class QuestsFactory
    {
        /// <summary>
        /// Builds a quest stage based on a prototype and stage data.
        /// </summary>
        /// <param name="prototypeStages">The repository of prototype stages.</param>
        /// <param name="stageData">The data for the quest stage.</param>
        /// <returns>The built quest stage.</returns>
        public static QuestStage BuildQuestStage(
            IReadOnlyRepository<string, QuestStage> prototypeStages,
            QuestStageDTO stageData)
        {
            return new QuestStage(
                prototypeStages.Get(stageData.ActionID),
                stageData.Properties);
        }

        /// <summary>
        /// Builds the prototype stages repository.
        /// </summary>
        /// <returns>The prototype stages repository.</returns>
        public static IReadOnlyRepository<string, QuestStage> BuildPrototypeStages()
        {
            IRepository<string, QuestStage> prototypeStages =
                RepositoriesFactory.BuildDictionaryRepository<string, QuestStage>();

            AddActions(prototypeStages);
            AddTrackers(prototypeStages);
            AddAwaiters(prototypeStages);
            //AddCheckpoints(prototypeStages);

            return (IReadOnlyRepository<string, QuestStage>)prototypeStages;
        }

        #region Actions

        /// <summary>
        /// Adds action stages to the prototype stages repository.
        /// </summary>
        /// <param name="prototypeStages">The prototype stages repository.</param>
        private static void AddActions(IRepository<string, QuestStage> prototypeStages)
        {
            prototypeStages.Add("NOP_ACTION", NOPAction());
            //prototypeStages.Add("CLOSE_ACTIVE_WINDOW", CloseActiveWindow());
        }

        /// <summary>
        /// Gets a "NOP_ACTION" quest stage.
        /// </summary>
        /// <returns>The "NOP_ACTION" quest stage.</returns>
        private static QuestStage NOPAction()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "NOP_ACTION",
                    StageType = EStageTypes.ACTION
                },
                (activeQuestManager, activeQuest) =>
                {
                    activeQuest.StageComplete();
                });
        }

        /*
        private static QuestStage CloseActiveWindow()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "CLOSE_ACTIVE_WINDOW",
                    StageType = EStageTypes.ACTION
                },
                (activeQuestManager, activeQuest) =>
                {
                    // messageBus
                    //     .PopMessage<PlayerClosedWindowEvent>(out var message)
                    //     .Write(message,false)
                    //     .SendImmediately(message);

                    activeQuest.StageComplete();
                });
        }
        */

        #endregion

        #region Trackers

        /// <summary>
        /// Adds tracker stages to the prototype stages repository.
        /// </summary>
        /// <param name="prototypeStages">The prototype stages repository.</param>
        private static void AddTrackers(IRepository<string, QuestStage> prototypeStages)
        {
            prototypeStages.Add("ENABLE_OBJECTIVE", EnableObjective());
            prototypeStages.Add("ENABLE_ALL_OBJECTIVES", EnableAllObjectives());
            prototypeStages.Add("DISABLE_OBJECTIVE", DisableObjective());
            prototypeStages.Add("DISABLE_ALL_OBJECTIVES", DisableAllObjectives());
        }

        /// <summary>
        /// Gets an "ENABLE_OBJECTIVE" quest stage.
        /// </summary>
        /// <returns>The "ENABLE_OBJECTIVE" quest stage.</returns>
        private static QuestStage EnableObjective()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "ENABLE_OBJECTIVE",
                    StageType = EStageTypes.TRACKER
                },
                (activeQuestManager, activeQuest) =>
                {
                    int objectiveIndex = activeQuest.CurrentStage.GetInt("OBJECTIVE_INDEX");

                    activeQuestManager.EnableQuestObjective(activeQuest, objectiveIndex);

                    activeQuest.StageComplete();
                });
        }

        /// <summary>
        /// Gets an "ENABLE_ALL_OBJECTIVES" quest stage.
        /// </summary>
        /// <returns>The "ENABLE_ALL_OBJECTIVES" quest stage.</returns>
        private static QuestStage EnableAllObjectives()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "ENABLE_ALL_OBJECTIVES",
                    StageType = EStageTypes.TRACKER
                },
                (activeQuestManager, activeQuest) =>
                {
                    activeQuestManager.EnableAllQuestObjectives(activeQuest);

                    activeQuest.StageComplete();
                });
        }

        /// <summary>
        /// Gets a "DISABLE_OBJECTIVE" quest stage.
        /// </summary>
        /// <returns>The "DISABLE_OBJECTIVE" quest stage.</returns>
        private static QuestStage DisableObjective()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "DISABLE_OBJECTIVE",
                    StageType = EStageTypes.TRACKER
                },
                (activeQuestManager, activeQuest) =>
                {
                    int objectiveIndex = activeQuest.CurrentStage.GetInt("OBJECTIVE_INDEX");

                    activeQuestManager.DisableQuestObjective(activeQuest, objectiveIndex);

                    activeQuest.StageComplete();
                });
        }

        /// <summary>
        /// Gets a "DISABLE_ALL_OBJECTIVES" quest stage.
        /// </summary>
        /// <returns>The "DISABLE_ALL_OBJECTIVES" quest stage.</returns>
        private static QuestStage DisableAllObjectives()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "DISABLE_ALL_OBJECTIVES",
                    StageType = EStageTypes.TRACKER
                },
                (activeQuestManager, activeQuest) =>
                {
                    activeQuestManager.DisableAllActiveObjectives(activeQuest);

                    activeQuest.StageComplete();
                });
        }

        #endregion

        #region Awaiters

        /// <summary>
        /// Adds awaiter stages to the prototype stages repository.
        /// </summary>
        /// <param name="prototypeStages">The prototype stages repository.</param>
        private static void AddAwaiters(IRepository<string, QuestStage> prototypeStages)
        {
            prototypeStages.Add("AWAIT_ALL_ACTIVE_OBJECTIVES_COMPLETE", AwaitAllActiveObjectivesComplete());
            prototypeStages.Add("DEBUG_AWAIT_NEVER", DebugAwaitNever());
        }

        /// <summary>
        /// Gets an "AWAIT_ALL_ACTIVE_OBJECTIVES_COMPLETE" quest stage.
        /// </summary>
        /// <returns>The "AWAIT_ALL_ACTIVE_OBJECTIVES_COMPLETE" quest stage.</returns>
        private static QuestStage AwaitAllActiveObjectivesComplete()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "AWAIT_ALL_ACTIVE_OBJECTIVES_COMPLETE",
                    StageType = EStageTypes.AWAITER
                },
                (activeQuestManager, activeQuest) =>
                {
                    Action cleanup = default(Action);

                    foreach (var tracker in activeQuest.QuestProgressTracker.AllActiveObjectives)
                    {
#if UNITY_EDITOR
                        int desiredValueClosure = tracker.Objective.Descriptor.ExpectedValue;

                        string trackerIDClosure = tracker.Objective.Descriptor.ObjectiveID;
#endif

                        Action<int> onValueChanged = (value) =>
                        {
#if UNITY_EDITOR
                            Debug.Log($"[QuestStageFactory] TRACKER VALUE MODIFIED: {trackerIDClosure} {value} / {desiredValueClosure}");
#endif

                            if (activeQuest.QuestProgressTracker.AllActiveObjectivesValidated)
                            {
                                cleanup?.Invoke();

                                activeQuest.StageComplete();
                            }
                        };

                        tracker.Progress.OnValueChanged += onValueChanged;

                        cleanup += (Action)(() =>
                        {
                            tracker.Progress.OnValueChanged -= onValueChanged;
                        });
                    }
                });
        }

        /// <summary>
        /// Gets a "DEBUG_AWAIT_NEVER" quest stage.
        /// </summary>
        /// <returns>The "DEBUG_AWAIT_NEVER" quest stage.</returns>
        private static QuestStage DebugAwaitNever()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "DEBUG_AWAIT_NEVER",
                    StageType = EStageTypes.AWAITER
                },
                (activeQuestManager, activeQuest) =>
                {
                });
        }

        #endregion

        #region Checkpoints

        /*
        private void AddCheckpoints(Dictionary<string, QuestStage> stages)
        {
            stages.Add("CHECKPOINT", Checkpoint());
        }

        private QuestStage Checkpoint()
        {
            return new QuestStage(
                "CHECKPOINT",
                EStageTypes.CHECKPOINT,
                (quest) =>
                {
                    quest.Checkpoint();

                    quest.StageComplete();
                });
        }
        */

        #endregion
    }
}