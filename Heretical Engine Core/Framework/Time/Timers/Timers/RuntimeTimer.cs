using HereticalSolutions.Delegates;

using HereticalSolutions.Persistence;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time.Timers
{
    public class RuntimeTimer
        : ITimer,
          IRuntimeTimer,
          IRuntimeTimerContext,
          ITimerWithState,
          ITickable,
          IVisitable
    {
        private readonly IReadOnlyRepository<ETimerState, ITimerStrategy<IRuntimeTimerContext, float>> strategyRepository;

        private readonly IFormatLogger logger;

        private ITimerStrategy<IRuntimeTimerContext, float> currentStrategy;
        

        public RuntimeTimer(
            string id,
            float defaultDuration,

            IPublisherSingleArgGeneric<IRuntimeTimer> onStartAsPublisher,
            INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> onStartAsSubscribable,

            IPublisherSingleArgGeneric<IRuntimeTimer> onFinishAsPublisher,
            INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> onFinishAsSubscribable,

            IReadOnlyRepository<ETimerState, ITimerStrategy<IRuntimeTimerContext, float>> strategyRepository,

            IFormatLogger logger)
        {
            ID = id;

            CurrentTimeElapsed = 0f;

            CurrentDuration = DefaultDuration = defaultDuration;


            OnStartAsPublisher = onStartAsPublisher;

            OnStart = onStartAsSubscribable;

            OnFinishAsPublisher = onFinishAsPublisher;

            OnFinish = onFinishAsSubscribable;


            this.strategyRepository = strategyRepository;

            this.logger = logger;

            SetState(ETimerState.INACTIVE);
        }

        #region IRuntimeTimerContext

        /// <summary>
        /// Gets or sets the current time elapsed.
        /// </summary>
        public float CurrentTimeElapsed { get; set; }

        /// <summary>
        /// Gets or sets the current duration of the timer.
        /// </summary>
        public float CurrentDuration { get; set; }

        /// <summary>
        /// Gets the default duration of the timer.
        /// </summary>
        public float DefaultDuration { get; private set; }

        /// <summary>
        /// Gets the publisher for the OnStart event.
        /// </summary>
        public IPublisherSingleArgGeneric<IRuntimeTimer> OnStartAsPublisher { get; private set; }

        /// <summary>
        /// Gets the publisher for the OnFinish event.
        /// </summary>
        public IPublisherSingleArgGeneric<IRuntimeTimer> OnFinishAsPublisher { get; private set; }

        #endregion

        #region ITimer

        /// <summary>
        /// Gets the ID of the timer.
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Gets the state of the timer.
        /// </summary>
        public ETimerState State { get; private set; }

        /// <summary>
        /// Gets the progress of the timer.
        /// </summary>
        public float Progress => currentStrategy.GetProgress(this);

        /// <summary>
        /// Gets or sets a value indicating whether the timer should accumulate time when paused.
        /// </summary>
        public bool Accumulate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the timer should repeat after finishing.
        /// </summary>
        public bool Repeat { get; set; }

        public bool FlushTimeElapsedOnRepeat { get; set; }

        /// <summary>
        /// Resets the timer.
        /// </summary>
        public void Reset()
        {
            currentStrategy.Reset(this);
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start()
        {
            currentStrategy.Start(this);
        }

        /// <summary>
        /// Pauses the timer.
        /// </summary>
        public void Pause()
        {
            currentStrategy.Pause(this);
        }

        /// <summary>
        /// Resumes the timer.
        /// </summary>
        public void Resume()
        {
            currentStrategy.Resume(this);
        }

        /// <summary>
        /// Aborts the timer.
        /// </summary>
        public void Abort()
        {
            currentStrategy.Abort(this);
        }

        /// <summary>
        /// Finishes the timer.
        /// </summary>
        public void Finish()
        {
            currentStrategy.Finish(this);
        }

        #endregion

        #region IRuntimeTimer

        /// <summary>
        /// Gets the time elapsed since the timer started.
        /// </summary>
        public float TimeElapsed => currentStrategy.GetTimeElapsed(this);

        /// <summary>
        /// Gets the remaining time until the timer finishes.
        /// </summary>
        public float Countdown => currentStrategy.GetCountdown(this);

        /// <summary>
        /// Resets the timer with the specified duration.
        /// </summary>
        /// <param name="duration">The new duration of the timer.</param>
        public void Reset(float duration)
        {
            Reset();
            CurrentDuration = duration;
        }

        /// <summary>
        /// Starts the timer with the specified duration.
        /// </summary>
        /// <param name="duration">The duration of the timer.</param>
        public void Start(float duration)
        {
            CurrentDuration = duration;
            Start();
        }

        /// <summary>
        /// Gets the subscribable for the OnStart event.
        /// </summary>
        public INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> OnStart { get; private set; }

        /// <summary>
        /// Gets the subscribable for the OnFinish event.
        /// </summary>
        public INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> OnFinish { get; private set; }

        #endregion

        #region ITickable

        /// <summary>
        /// Performs a tick on the timer.
        /// </summary>
        /// <param name="delta">The time since the last tick.</param>
        public void Tick(float delta)
        {
            currentStrategy.Tick(this, delta);
        }

        #endregion

        #region ITimerWithState

        /// <summary>
        /// Sets the state of the timer.
        /// </summary>
        /// <param name="state">The state to set.</param>
        public void SetState(ETimerState state)
        {
            State = state;
            // Strategies should exist for all states in enum therefore Get
            currentStrategy = strategyRepository.Get(state);
        }

        #endregion

        #region IVisitable

        /// <summary>
        /// Gets the type of the DTO object for serialization.
        /// </summary>
        public Type DTOType => typeof(RuntimeTimerDTO);

        public bool Accept<TDTO>(
            ISaveVisitor visitor,
            out TDTO DTO)
        {
            var result = visitor
                .Save<IRuntimeTimer, RuntimeTimerDTO>(
                    this,
                    out RuntimeTimerDTO runtimeTimerDTO);

            DTO = default;

            //LOL, pattern matching to the rescue of converting TArgument to TValue
            switch (runtimeTimerDTO)
            {
                case TDTO targetTypeDTO:

                    DTO = targetTypeDTO;

                    break;

                default:

                    logger?.ThrowException<RuntimeTimer>(
                        $"CANNOT CAST RETURN VALUE TYPE \"{typeof(RuntimeTimerDTO).Name}\" TO TYPE \"{typeof(TDTO).GetType().Name}\"");

                    break;
            }

            return result;
        }

        /// <summary>
        /// Accepts a save visitor and converts the timer to a DTO object.
        /// </summary>
        /// <param name="visitor">The save visitor to accept.</param>
        /// <param name="DTO">The converted DTO object.</param>
        /// <returns>A value indicating whether the conversion was successful.</returns>
        public bool Accept(ISaveVisitor visitor, out object DTO)
        {
            var result = visitor.Save<IRuntimeTimer, RuntimeTimerDTO>(this, out RuntimeTimerDTO runtimeTimerDTO);

            DTO = runtimeTimerDTO;

            return result;
        }

        public bool Accept<TDTO>(
            ILoadVisitor visitor,
            TDTO DTO)
        {
            //LOL, pattern matching to the rescue of converting TArgument to TValue
            switch (DTO)
            {
                case RuntimeTimerDTO targetTypeDTO:

                    return visitor
                        .Load<IRuntimeTimer, RuntimeTimerDTO>(
                            targetTypeDTO,
                            this);

                default:

                    logger?.ThrowException<RuntimeTimer>(
                        $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).Name}\" RECEIVED: \"{typeof(TDTO).GetType().Name}\"");

                    return false;
            }
        }

        /// <summary>
        /// Accepts a load visitor and applies the values from the DTO object to the timer.
        /// </summary>
        /// <param name="visitor">The load visitor to accept.</param>
        /// <param name="DTO">The DTO object containing the values to apply.</param>
        /// <returns>A value indicating whether the application of values was successful.</returns>
        public bool Accept(ILoadVisitor visitor, object DTO)
        {
            return visitor.Load<IRuntimeTimer, RuntimeTimerDTO>((RuntimeTimerDTO)DTO, this);
        }

        #endregion
    }
}