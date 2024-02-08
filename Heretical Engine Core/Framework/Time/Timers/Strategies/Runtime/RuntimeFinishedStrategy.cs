namespace HereticalSolutions.Time.Strategies
{
    /// <summary>
    /// Represents a strategy for a runtime finished timer
    /// </summary>
    public class RuntimeFinishedStrategy : ITimerStrategy<IRuntimeTimerContext, float>
    {
        #region Progress
        
        /// <summary>
        /// Gets the progress of the timer
        /// </summary>
        /// <param name="context">The timer context.</param>
        /// <returns>The progress of the timer.</returns>
        /// <remarks>
        /// This method returns 0 if the timer was finished prematurely by a <see cref="Finish"/> call rather than the timer actually running out
        /// </remarks>
        public float GetProgress(IRuntimeTimerContext context)
        {
            //THIS ONE IS AS EXPECTED. IF THE TIMER WAS FINISHED PREMATURELY BY A FINISH() CALL RATHER THAN TIMER ACTUALLY RUNNING OUT WE MIGHT BE CURIOUS HOW MUCH OF A PROGRESS WAS MADE SO FAR
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
        /// <param name="context">The timer context.</param>
        /// <returns>The time elapsed in seconds.</returns>
        public float GetTimeElapsed(IRuntimeTimerContext context)
        {
            return context.CurrentTimeElapsed;
        }
        
        /// <summary>
        /// Gets the time remaining until the timer finishes
        /// </summary>
        /// <param name="context">The timer context.</param>
        /// <returns>The time remaining in seconds.</returns>
        public float GetCountdown(IRuntimeTimerContext context)
        {
            return context.CurrentDuration - context.CurrentTimeElapsed;
        }
        
        #endregion
        
        #region Controls
        
        /// <summary>
        /// Resets the timer by setting the time elapsed to zero and the duration to the default duration
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Reset(IRuntimeTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;

            context.CurrentDuration = context.DefaultDuration;
            
            context.SetState(ETimerState.INACTIVE);
        }

        /// <summary>
        /// Starts the timer by setting the time elapsed to zero and changing the timer state to started
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Start(IRuntimeTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;
            
            context.SetState(ETimerState.STARTED);
            
            context.OnStartAsPublisher.Publish((IRuntimeTimer)context);
        }

        /// <summary>
        /// Pauses the timer. This method does nothing
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Pause(IRuntimeTimerContext context)
        {
            //Why bother?
        }

        /// <summary>
        /// Resumes the timer. This method does nothing
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Resume(IRuntimeTimerContext context)
        {
            //Why bother?
        }
        
        /// <summary>
        /// Aborts the timer by setting the time elapsed to zero and changing the timer state to inactive
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Abort(IRuntimeTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;
            
            context.SetState(ETimerState.INACTIVE);
        }
        
        /// <summary>
        /// Finishes the timer. This method does nothing
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Finish(IRuntimeTimerContext context)
        {
            //Why bother?
        }

        /// <summary>
        /// Advances the timer. This method does nothing
        /// </summary>
        /// <param name="context">The timer context.</param>
        /// <param name="delta">The time in seconds since the last tick.</param>
        public void Tick(IRuntimeTimerContext context, float delta)
        {
            //Why bother?
        }
        
        #endregion
    }
}