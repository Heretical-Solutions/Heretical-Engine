using HereticalSolutions.Persistence;

using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time.Visitors
{
    public class PersistentTimerVisitor
        : ASaveLoadVisitor<IPersistentTimer, PersistentTimerDTO>
    {
        private readonly ILoggerResolver loggerResolver;

        public PersistentTimerVisitor(
            ILoggerResolver loggerResolver = null,
            ILogger logger = null)
            : base(logger)
        {
            this.loggerResolver = loggerResolver;
        }

        #region ILoadVisitorGeneric

        public override bool Load(
            PersistentTimerDTO DTO,
            out IPersistentTimer value)
        {
            value = TimeFactory.BuildPersistentTimer(
                DTO.ID,
                DTO.DefaultDurationSpan,
                loggerResolver);

            ((ITimerWithState)value).SetState(DTO.State);

            ((IPersistentTimerContext)value).StartTime = DTO.StartTime;

            ((IPersistentTimerContext)value).EstimatedFinishTime = DTO.EstimatedFinishTime;

            ((IPersistentTimerContext)value).SavedProgress = DTO.SavedProgress;

            ((IPersistentTimerContext)value).CurrentDurationSpan = DTO.CurrentDurationSpan;

            value.Accumulate = DTO.Accumulate;

            value.Repeat = DTO.Repeat;

            return true;
        }

        public override bool Load(
            PersistentTimerDTO DTO,
            IPersistentTimer valueToPopulate)
        {
            ((ITimerWithState)valueToPopulate).SetState(DTO.State);

            ((IPersistentTimerContext)valueToPopulate).StartTime = DTO.StartTime;

            ((IPersistentTimerContext)valueToPopulate).EstimatedFinishTime = DTO.EstimatedFinishTime;

            ((IPersistentTimerContext)valueToPopulate).SavedProgress = DTO.SavedProgress;

            ((IPersistentTimerContext)valueToPopulate).CurrentDurationSpan = DTO.CurrentDurationSpan;

            valueToPopulate.Accumulate = DTO.Accumulate;

            valueToPopulate.Repeat = DTO.Repeat;

            return true;
        }

        #endregion

        #region ISaveVisitorGeneric

        public override bool Save(
            IPersistentTimer value,
            out PersistentTimerDTO DTO)
        {
            DTO = new PersistentTimerDTO
            {
                ID = value.ID,
                State = value.State,
                StartTime = ((IPersistentTimerContext)value).StartTime,
                EstimatedFinishTime = ((IPersistentTimerContext)value).EstimatedFinishTime,
                SavedProgress = ((IPersistentTimerContext)value).SavedProgress,
                Accumulate = value.Accumulate,
                Repeat = value.Repeat,
                CurrentDurationSpan = value.CurrentDurationSpan,
                DefaultDurationSpan = value.DefaultDurationSpan
            };

            return true;
        }

        #endregion
    }
}