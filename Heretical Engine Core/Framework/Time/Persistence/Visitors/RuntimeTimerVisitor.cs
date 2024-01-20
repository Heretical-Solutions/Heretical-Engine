using HereticalSolutions.Persistence;

using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time.Visitors
{
    public class RuntimeTimerVisitor
        : ASaveLoadVisitor<IRuntimeTimer, RuntimeTimerDTO>
    {
        private readonly ILoggerResolver loggerResolver;

        public RuntimeTimerVisitor(
            ILoggerResolver loggerResolver = null,
            ILogger logger = null)
            : base(logger)
        {
            this.loggerResolver = loggerResolver;
        }

        #region ILoadVisitorGeneric

        public override bool Load(RuntimeTimerDTO DTO, out IRuntimeTimer value)
        {
            value = TimeFactory.BuildRuntimeTimer(
                DTO.ID,
                DTO.DefaultDuration,
                loggerResolver);

            ((ITimerWithState)value).SetState(DTO.State);

            ((IRuntimeTimerContext)value).CurrentTimeElapsed = DTO.CurrentTimeElapsed;

            ((IRuntimeTimerContext)value).CurrentDuration = DTO.CurrentDuration;

            value.Accumulate = DTO.Accumulate;

            value.Repeat = DTO.Repeat;

            return true;
        }

        public override bool Load(RuntimeTimerDTO DTO, IRuntimeTimer valueToPopulate)
        {
            ((ITimerWithState)valueToPopulate).SetState(DTO.State);

            ((IRuntimeTimerContext)valueToPopulate).CurrentTimeElapsed = DTO.CurrentTimeElapsed;

            ((IRuntimeTimerContext)valueToPopulate).CurrentDuration = DTO.CurrentDuration;

            valueToPopulate.Accumulate = DTO.Accumulate;

            valueToPopulate.Repeat = DTO.Repeat;

            return true;
        }

        #endregion

        #region ISaveVisitorGeneric

        public override bool Save(IRuntimeTimer value, out RuntimeTimerDTO DTO)
        {
            DTO = new RuntimeTimerDTO
            {
                ID = value.ID,
                State = value.State,
                CurrentTimeElapsed = ((IRuntimeTimerContext)value).CurrentTimeElapsed,
                Accumulate = value.Accumulate,
                Repeat = value.Repeat,
                CurrentDuration = value.CurrentDuration,
                DefaultDuration = value.DefaultDuration
            };

            return true;
        }

        #endregion
    }
}