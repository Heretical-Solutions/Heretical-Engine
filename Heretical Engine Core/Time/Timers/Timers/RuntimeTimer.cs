using System;
using HereticalSolutions.Delegates;
using HereticalSolutions.Persistence;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.Time.Timers
{
    /// <summary>
    /// Represents a runtime timer that can be used for tracking time and executing actions based on the elapsed time.
    /// </summary>
    public class RuntimeTimer : ITimer, IRuntimeTimer, IRuntimeTimerContext, ITimerWithState, ITickable, IVisitable
    {
        private ITimerStrategy<IRuntimeTimerContext, float> currentStrategy;
        private readonly IReadOnlyRepository<ETimerState, ITimerStrategy<IRuntimeTimerContext, float>> strategyRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeTimer"/> class.
        /// </summary>
        /// <param name="id">The ID of the timer.</param>
        /// <param name="defaultDuration">The default duration of the timer.</param>
        /// <param name="onStartAsPublisher">The publisher for the OnStart event.</param>
        /// <param name="onStartAsSubscribable">The subscribable for the OnStart event.</param>
        /// <param name="onFinishAsPublisher">The publisher for the OnFinish event.</param>
        /// <param name="onFinishAsSubscribable">The subscribable for the OnFinish event.</param>
        /// <param name="strategyRepository">The repository containing the timer strategies.</param>
        public RuntimeTimer(
            string id,
            float defaultDuration,
            IPublisherSingleArgGeneric<IRuntimeTimer> onStartAsPublisher,
            INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> onStartAsSubscribable,
            IPublisherSingleArgGeneric<IRuntimeTimer> onFinishAsPublisher,
            INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> onFinishAsSubscribable,
            IReadOnlyRepository<ETimerState, ITimerStrategy<IRuntimeTimerContext, float>> strategyRepository)
        {
            ID = id;
            CurrentTimeElapsed = 0f;
            CurrentDuration = DefaultDuration = defaultDuration;
            OnStartAsPublisher = onStartAsPublisher;
            OnStart = onStartAsSubscribable;
            OnFinishAsPublisher = onFinishAsPublisher;
            OnFinish = onFinishAsSubscribable;
            this.strategyRepository = strategyRepository;
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

        /// <summary>
        /// Accepts a save visitor and converts the timer to a DTO object.
        /// </summary>
        /// <typeparam name="TDTO">The type of the DTO object.</typeparam>
        /// <param name="visitor">The save visitor to accept.</param>
        /// <param name="DTO">The converted DTO object.</param>
        /// <returns>A value indicating whether the conversion was successful.</returns>
        public bool Accept<TDTO>(ISaveVisitor visitor, out TDTO DTO)
        {
            if (!(typeof(TDTO).Equals(typeof(RuntimeTimerDTO))))
                throw new Exception($"[RuntimeTimer] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");

            var result = visitor.Save<IRuntimeTimer, RuntimeTimerDTO>(this, out RuntimeTimerDTO runtimeTimerDTO);

            // DIRTY HACKS DO NOT REPEAT
            var dtoObject = (object)runtimeTimerDTO;

            DTO = (TDTO)dtoObject;

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

        /// <summary>
        /// Accepts a load visitor and applies the values from the DTO object to the timer.
        /// </summary>
        /// <typeparam name="TDTO">The type of the DTO object.</typeparam>
        /// <param name="visitor">The load visitor to accept.</param>
        /// <param name="DTO">The DTO object containing the values to apply.</param>
        /// <returns>A value indicating whether the application of values was successful.</returns>
        public bool Accept<TDTO>(ILoadVisitor visitor, TDTO DTO)
        {
            if (!(typeof(TDTO).Equals(typeof(RuntimeTimerDTO))))
                throw new Exception($"[RuntimeTimer] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");

            // DIRTY HACKS DO NOT REPEAT
            var dtoObject = (object)DTO;

            return visitor.Load<IRuntimeTimer, RuntimeTimerDTO>((RuntimeTimerDTO)dtoObject, this);
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