using HereticalSolutions.MVVM;
using HereticalSolutions.MVVM.Observable;

namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Represents an active quest objective
    /// </summary>
    public class ActiveQuestObjective
    {
        /// <summary>
        /// Gets the quest objective associated with this active quest objective
        /// </summary>
        public QuestObjective Objective { get; private set; }

        /// <summary>
        /// Gets the progress of the active quest objective
        /// </summary>
        public IObservableProperty<int> Progress { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ActiveQuestObjective class
        /// </summary>
        /// <param name="objective">The quest objective.</param>
        /// <param name="progress">The progress of the objective.</param>
        public ActiveQuestObjective(
            QuestObjective objective, 
            ObservableProperty<int> progress)
        {
            Objective = objective;
            Progress = progress;
        }

        /// <summary>
        /// Modifies the progress of the active quest objective based on the specified operation and value
        /// </summary>
        /// <param name="operation">The operation to perform on the progress.</param>
        /// <param name="value">The value to modify the progress by.</param>
        public void Modify(ETrackerOperation operation, int value)
        {
            switch (operation)
            {
                case ETrackerOperation.SET_TO_VALUE:
                    Progress.Value = value;
                    break;
                
                case ETrackerOperation.INCREMENT:
                    Progress.Value++;
                    break;
                
                case ETrackerOperation.DECREMENT:
                    Progress.Value--;
                    break;
                    
                case ETrackerOperation.ADD:
                    Progress.Value += value;
                    break;
                
                case ETrackerOperation.SUBSTRACT:
                    Progress.Value -= value;
                    break;
                
                case ETrackerOperation.MULTIPLY:
                    Progress.Value *= value;
                    break;
                
                case ETrackerOperation.DIVIDE:
                    Progress.Value = (int)((float)Progress.Value / (float)value);
                    break;
            }
        }

        /// <summary>
        /// Validates the active quest objective based on the comparison specified by the objective descriptor
        /// </summary>
        /// <returns>True if the active quest objective is valid, otherwise false.</returns>
        public bool Validate()
        {
            switch (Objective.Descriptor.Comparison)
            {
                case ETrackerComparison.EQUALS_TO:
                    return Progress.Value == Objective.Descriptor.ExpectedValue;
                
                case ETrackerComparison.NOT_EQUALS_TO:
                    return Progress.Value != Objective.Descriptor.ExpectedValue;
                
                case ETrackerComparison.MORE_THAN:
                    return Progress.Value > Objective.Descriptor.ExpectedValue;
                
                case ETrackerComparison.LESS_THAN:
                    return Progress.Value < Objective.Descriptor.ExpectedValue;
                
                case ETrackerComparison.EQUALS_OR_MORE_THAN:
                    return Progress.Value >= Objective.Descriptor.ExpectedValue;
                
                case ETrackerComparison.EQUALS_OR_LESS_THAN:
                    return Progress.Value <= Objective.Descriptor.ExpectedValue;
            }
            
            throw new Exception("[QuestObjectiveTracker] INVALID OPERATION");
        }
    }
}