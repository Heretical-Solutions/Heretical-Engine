namespace HereticalSolutions.Time.Strategies
{
    /// <summary>
    /// Represents a strategy for an inactive runtime timer
    /// </summary>
    public class RuntimeInactiveStrategy : ITimerStrategy<IRuntimeTimerContext, float>
    {
        #region Progress

        /// <summary>
        /// Gets the progress of the timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        /// <returns>The progress of the timer.</returns>
        public float GetProgress(IRuntimeTimerContext context)
        {
            return 0f;
        }

        #endregion

        #region Countdown and Time elapsed

        /// <summary>
        /// Gets the time elapsed since the timer started
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        /// <returns>The time elapsed since the timer started.</returns>
        public float GetTimeElapsed(IRuntimeTimerContext context)
        {
            return 0f;
        }

        /// <summary>
        /// Gets the remaining time in the countdown timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        /// <returns>The remaining time in the countdown timer.</returns>
        public float GetCountdown(IRuntimeTimerContext context)
        {
            return 0f;
        }

        #endregion

        #region Controls

        /// <summary>
        /// Resets the timer to its initial state
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Reset(IRuntimeTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;
            context.CurrentDuration = context.DefaultDuration;
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Start(IRuntimeTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;
            context.SetState(ETimerState.STARTED);
            context.OnStartAsPublisher.Publish((IRuntimeTimer)context);
        }

        /// <summary>
        /// Pauses the timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Pause(IRuntimeTimerContext context)
        {
            //Why bother?
        }

        /// <summary>
        /// Resumes the timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Resume(IRuntimeTimerContext context)
        {
            //Why bother?
        }

        /// <summary>
        /// Aborts the timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Abort(IRuntimeTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;
        }

        /// <summary>
        /// Finishes the timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Finish(IRuntimeTimerContext context)
        {
            //ENSURE WHETHER CALLING FINISH() ON INACTIVE TIMER SHOULD NOT BE CALLING A CALLBACK
        }

        /// <summary>
        /// Updates the timer with the specified delta
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        /// <param name="delta">The time interval since the last update.</param>
        public void Tick(IRuntimeTimerContext context, float delta)
        {
            //Why bother?
        }

        #endregion
    }
}