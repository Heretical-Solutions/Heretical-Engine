using HereticalSolutions.Delegates;

using HereticalSolutions.Persistence;

using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time.Timers
{
    public class PersistentTimer
        : ITimer,
          IPersistentTimer,
          IPersistentTimerContext,
          ITimerWithState,
          ITickable,
          IVisitable,
          ICleanUppable,
          IDisposable
    {
        private readonly IReadOnlyRepository<ETimerState, ITimerStrategy<IPersistentTimerContext, TimeSpan>> strategyRepository;

        private readonly ILogger logger;

        private ITimerStrategy<IPersistentTimerContext, TimeSpan> currentStrategy;

        public PersistentTimer(
            string id,
            TimeSpan defaultDurationSpan,

            IPublisherSingleArgGeneric<IPersistentTimer> onStartAsPublisher,
            INonAllocSubscribableSingleArgGeneric<IPersistentTimer> onStartAsSubscribable,

            IPublisherSingleArgGeneric<IPersistentTimer> onFinishAsPublisher,
            INonAllocSubscribableSingleArgGeneric<IPersistentTimer> onFinishAsSubscribable,

            IReadOnlyRepository<ETimerState, ITimerStrategy<IPersistentTimerContext, TimeSpan>> strategyRepository,

            ILogger logger = null)
        {
            ID = id;

            StartTime = default(DateTime);

            EstimatedFinishTime = default(DateTime);

            SavedProgress = default(TimeSpan);

            CurrentDurationSpan = DefaultDurationSpan = defaultDurationSpan;


            OnStartAsPublisher = onStartAsPublisher;

            OnStart = onStartAsSubscribable;


            OnFinishAsPublisher = onFinishAsPublisher;

            OnFinish = onFinishAsSubscribable;


            this.strategyRepository = strategyRepository;

            this.logger = logger;

            SetState(ETimerState.INACTIVE);
        }

        #region IPersistentTimerContext

        #region Variables

        /// <summary>
        /// Gets or sets the start time of the timer
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the estimated finish time of the timer
        /// </summary>
        public DateTime EstimatedFinishTime { get; set; }

        /// <summary>
        /// Gets or sets the saved progress of the timer
        /// </summary>
        public TimeSpan SavedProgress { get; set; }

        #endregion

        #region Duration

        /// <summary>
        /// Gets or sets the current duration span of the timer
        /// </summary>
        public TimeSpan CurrentDurationSpan { get; set; }

        /// <summary>
        /// Gets or sets the default duration span of the timer
        /// </summary>
        public TimeSpan DefaultDurationSpan { get; set; }

        #endregion

        #region Publishers

        /// <summary>
        /// Gets the publisher for the "on start" event of the timer
        /// </summary>
        public IPublisherSingleArgGeneric<IPersistentTimer> OnStartAsPublisher { get; private set; }

        /// <summary>
        /// Gets the subscribable for the "on finish" event of the timer
        /// </summary>
        public IPublisherSingleArgGeneric<IPersistentTimer> OnFinishAsPublisher { get; private set; }

        #endregion

        #endregion

        #region ITimer

        /// <summary>
        /// Gets the ID of the timer
        /// </summary>
        public string ID { get; private set; }

        #region Timer state

        /// <summary>
        /// Gets the state of the timer
        /// </summary>
        public ETimerState State { get; private set; }

        #endregion

        #region Progress

        /// <summary>
        /// Gets the progress of the timer
        /// </summary>
        public float Progress
        {
            get => currentStrategy.GetProgress(this);
        }

        #endregion

        #region Controls

        /// <summary>
        /// Gets or sets a value indicating whether the timer should accumulate time when paused
        /// </summary>
        public bool Accumulate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the timer should repeat after finishing
        /// </summary>
        public bool Repeat { get; set; }

        public bool FlushTimeElapsedOnRepeat { get; set; }

        /// <summary>
        /// Resets the timer
        /// </summary>
        public void Reset()
        {
            currentStrategy.Reset(this);
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        public void Start()
        {
            currentStrategy.Start(this);
        }

        /// <summary>
        /// Pauses the timer
        /// </summary>
        public void Pause()
        {
            currentStrategy.Pause(this);
        }

        /// <summary>
        /// Resumes the timer
        /// </summary>
        public void Resume()
        {
            currentStrategy.Resume(this);
        }

        /// <summary>
        /// Aborts the timer
        /// </summary>
        public void Abort()
        {
            currentStrategy.Abort(this);
        }

        /// <summary>
        /// Finishes the timer
        /// </summary>
        public void Finish()
        {
            currentStrategy.Finish(this);
        }

        #endregion

        #endregion

        #region IPersistentTimer

        #region Countdown and Time elapsed

        /// <summary>
        /// Gets the time elapsed span of the timer
        /// </summary>
        public TimeSpan TimeElapsedSpan
        {
            get => currentStrategy.GetTimeElapsed(this);
        }

        /// <summary>
        /// Gets the countdown span of the timer
        /// </summary>
        public TimeSpan CountdownSpan
        {
            get => currentStrategy.GetCountdown(this);
        }

        #endregion

        #region Controls

        /// <summary>
        /// Resets the timer with a specified duration span
        /// </summary>
        /// <param name="durationSpan">The duration span to set for the timer.</param>
        public void Reset(TimeSpan durationSpan)
        {
            Reset();

            CurrentDurationSpan = durationSpan;
        }

        /// <summary>
        /// Starts the timer with a specified duration span
        /// </summary>
        /// <param name="durationSpan">The duration span to set for the timer.</param>
        public void Start(TimeSpan durationSpan)
        {
            CurrentDurationSpan = durationSpan;

            Start();
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Gets the subscribable for the "on start" event of the timer
        /// </summary>
        public INonAllocSubscribableSingleArgGeneric<IPersistentTimer> OnStart { get; private set; }

        /// <summary>
        /// Gets the subscribable for the "on finish" event of the timer
        /// </summary>
        public INonAllocSubscribableSingleArgGeneric<IPersistentTimer> OnFinish { get; private set; }

        #endregion

        #endregion

        #region ITickable

        /// <summary>
        /// Ticks the timer
        /// </summary>
        /// <param name="delta">The time difference.</param>
        public void Tick(float delta)
        {
            currentStrategy.Tick(this, delta);
        }

        #endregion

        #region ITimerWithState

        /// <summary>
        /// Sets the state of the timer
        /// </summary>
        /// <param name="state">The state to set.</param>
        public void SetState(ETimerState state)
        {
            State = state;

            // Strategies should exist for all states in enum, therefore get the corresponding strategy
            currentStrategy = strategyRepository.Get(state);
        }

        #endregion

        #region IVisitable

        /// <summary>
        /// Gets the type of the data transfer object for the timer
        /// </summary>
        public Type DTOType
        {
            get => typeof(PersistentTimerDTO);
        }

        public bool Accept<TDTO>(
            ISaveVisitor visitor,
            out TDTO DTO)
        {
            var result = visitor
                .Save<IPersistentTimer, PersistentTimerDTO>(
                    this,
                    out PersistentTimerDTO persistentTimerDTO);

            DTO = default;

            //LOL, pattern matching to the rescue of converting TArgument to TValue
            switch (persistentTimerDTO)
            {
                case TDTO targetTypeDTO:

                    DTO = targetTypeDTO;

                    break;

                default:

                    throw new Exception(
                        logger.TryFormat<PersistentTimer>(
                            $"CANNOT CAST RETURN VALUE TYPE \"{typeof(PersistentTimerDTO).Name}\" TO TYPE \"{typeof(TDTO).GetType().Name}\""));
            }

            return result;
        }

        /// <summary>
        /// Accepts a save visitor and returns whether the visit was successful
        /// </summary>
        /// <param name="visitor">The save visitor.</param>
        /// <param name="DTO">The data transfer object.</param>
        /// <returns>True if the visit was successful, false otherwise.</returns>
        public bool Accept(ISaveVisitor visitor, out object DTO)
        {
            var result = visitor.Save<IPersistentTimer, PersistentTimerDTO>(this, out PersistentTimerDTO persistentTimerDTO);

            DTO = persistentTimerDTO;

            return result;
        }

        public bool Accept<TDTO>(
            ILoadVisitor visitor,
            TDTO DTO)
        {
            //LOL, pattern matching to the rescue of converting TArgument to TValue
            switch (DTO)
            {
                case PersistentTimerDTO targetTypeDTO:

                    return visitor
                        .Load<IPersistentTimer, PersistentTimerDTO>(
                            targetTypeDTO,
                            this);

                default:

                    throw new Exception(
                        logger.TryFormat<PersistentTimer>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(PersistentTimerDTO).Name}\" RECEIVED: \"{typeof(TDTO).GetType().Name}\""));
            }
        }

        /// <summary>
        /// Accepts a load visitor and returns whether the visit was successful
        /// </summary>
        /// <param name="visitor">The load visitor.</param>
        /// <param name="DTO">The data transfer object.</param>
        /// <returns>True if the visit was successful, false otherwise.</returns>
        public bool Accept(ILoadVisitor visitor, object DTO)
        {
            return visitor.Load<IPersistentTimer, PersistentTimerDTO>((PersistentTimerDTO)DTO, this);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (OnStartAsPublisher is ICleanUppable)
                (OnStartAsPublisher as ICleanUppable).Cleanup();

            if (OnFinishAsPublisher is ICleanUppable)
                (OnFinishAsPublisher as ICleanUppable).Cleanup();

            if (strategyRepository is ICleanUppable)
                (strategyRepository as ICleanUppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (OnStartAsPublisher is IDisposable)
                (OnStartAsPublisher as IDisposable).Dispose();

            if (OnFinishAsPublisher is IDisposable)
                (OnFinishAsPublisher as IDisposable).Dispose();

            if (strategyRepository is IDisposable)
                (strategyRepository as IDisposable).Dispose();
        }

        #endregion
    }
}