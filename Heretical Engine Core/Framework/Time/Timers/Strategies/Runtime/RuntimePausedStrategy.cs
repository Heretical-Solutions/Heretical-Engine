namespace HereticalSolutions.Time.Strategies
{
    public class RuntimePausedStrategy : ITimerStrategy<IRuntimeTimerContext, float>
    {
        #region Progress
        
        /// <summary>
        /// Gets the progress of the timer as a value between 0 and 1, inclusive
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        /// <returns>The progress of the timer.</returns>
        public float GetProgress(IRuntimeTimerContext context)
        {
            if (context.Accumulate)
                return 0f;

            if ((context.CurrentDuration - MathHelpers.EPSILON) < 0f)
                return 0f;

            return (context.CurrentTimeElapsed / context.CurrentDuration).Clamp(0f, 1f);
        }

        #endregion

        #region Countdown and Time elapsed

        /// <summary>
        /// Gets the time elapsed since the timer started
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        /// <returns>The time elapsed in seconds.</returns>
        public float GetTimeElapsed(IRuntimeTimerContext context)
        {
            return context.CurrentTimeElapsed;
        }

        /// <summary>
        /// Gets the remaining time until the timer finishes
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        /// <returns>The remaining time in seconds.</returns>
        public float GetCountdown(IRuntimeTimerContext context)
        {
            return context.CurrentDuration - context.CurrentTimeElapsed;
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
            context.SetState(ETimerState.INACTIVE);
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Start(IRuntimeTimerContext context)
        {
            //ENSURE THAT CALLING START() ON A PAUSED TIMER SHOULD IGNORE A CALL INSTEAD OF RESETTING THE TIMER
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
            context.SetState(ETimerState.STARTED);
        }

        /// <summary>
        /// Aborts the timer and resets it to its initial state
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Abort(IRuntimeTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;
            context.SetState(ETimerState.INACTIVE);
        }

        /// <summary>
        /// Finishes the timer and publishes the finish event
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Finish(IRuntimeTimerContext context)
        {
            context.SetState(ETimerState.FINISHED);
            context.OnFinishAsPublisher.Publish((IRuntimeTimer)context);
        }

        /// <summary>
        /// Performs a tick action for the timer. No action is performed as the timer is paused
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        /// <param name="delta">The time passed since the last tick.</param>
        public void Tick(IRuntimeTimerContext context, float delta)
        {
            //Why bother?
        }

        #endregion
    }
}