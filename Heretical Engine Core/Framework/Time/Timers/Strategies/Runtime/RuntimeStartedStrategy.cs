namespace HereticalSolutions.Time.Strategies
{
    /// <summary>
    /// Represents a strategy for a runtime timer that starts automatically when the application starts
    /// </summary>
    public class RuntimeStartedStrategy : ITimerStrategy<IRuntimeTimerContext, float>
    {
        #region Progress
        
        /// <summary>
        /// Gets the progress of the runtime timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        /// <returns>The progress of the runtime timer as a value between 0 and 1.</returns>
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
        /// Gets the time elapsed since the runtime timer started
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        /// <returns>The time elapsed since the runtime timer started.</returns>
        public float GetTimeElapsed(IRuntimeTimerContext context)
        {
            return context.CurrentTimeElapsed;
        }

        /// <summary>
        /// Gets the remaining time until the runtime timer finishes
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        /// <returns>The remaining time until the runtime timer finishes.</returns>
        public float GetCountdown(IRuntimeTimerContext context)
        {
            return context.CurrentDuration - context.CurrentTimeElapsed;
        }
        
        #endregion
        
        #region Controls

        /// <summary>
        /// Resets the runtime timer to its initial state
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Reset(IRuntimeTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;

            context.CurrentDuration = context.DefaultDuration;
            
            context.SetState(ETimerState.INACTIVE);
        }

        /// <summary>
        /// Starts the runtime timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Start(IRuntimeTimerContext context)
        {
            //ENSURE THAT CALLING START() ON A RUNNING TIMER SHOULD IGNORE A CALL INSTEAD OF RESETTING THE TIMER
        }

        /// <summary>
        /// Pauses the runtime timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Pause(IRuntimeTimerContext context)
        {
            context.SetState(ETimerState.PAUSED);
        }

        /// <summary>
        /// Resumes the paused runtime timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Resume(IRuntimeTimerContext context)
        {
            //Why bother?
        }
        
        /// <summary>
        /// Aborts and resets the runtime timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Abort(IRuntimeTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;
            
            context.SetState(ETimerState.INACTIVE);
        }
        
        /// <summary>
        /// Finishes the runtime timer
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        public void Finish(IRuntimeTimerContext context)
        {
            context.SetState(ETimerState.FINISHED);
            
            context.OnFinishAsPublisher.Publish((IRuntimeTimer)context);
        }

        /// <summary>
        /// Advances the runtime timer by a specified amount of time
        /// </summary>
        /// <param name="context">The runtime timer context.</param>
        /// <param name="delta">The amount of time to advance the runtime timer.</param>
        public void Tick(IRuntimeTimerContext context, float delta)
        {
            context.CurrentTimeElapsed += delta;

            if (context.Accumulate)
            {
                return;
            }

            if (context.CurrentTimeElapsed > context.CurrentDuration)
            {
                if (context.Repeat)
                {
                    if (context.FlushTimeElapsedOnRepeat)
                    {
                        context.OnFinishAsPublisher.Publish((IRuntimeTimer)context);
                        
                        context.CurrentTimeElapsed = 0f;
                
                        context.OnStartAsPublisher.Publish((IRuntimeTimer)context);
                    }
                    else
                    {
                        while (context.CurrentTimeElapsed > context.CurrentDuration)
                        {
                            context.OnFinishAsPublisher.Publish((IRuntimeTimer)context);

                            context.CurrentTimeElapsed -= context.CurrentDuration;

                            context.OnStartAsPublisher.Publish((IRuntimeTimer)context);
                        }
                    }
                }
                else
                    Finish(context);
            }
        }
        
        #endregion
    }
}