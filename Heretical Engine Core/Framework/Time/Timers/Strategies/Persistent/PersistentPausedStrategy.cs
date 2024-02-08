using System;

namespace HereticalSolutions.Time.Strategies
{
    /// <summary>
    /// Represents a timer strategy for persistent timers with ability to pause and resume
    /// </summary>
    public class PersistentPausedStrategy : ITimerStrategy<IPersistentTimerContext, TimeSpan>
    {
        #region Progress
        
        /// <summary>
        /// Gets the progress of the timer as a value between 0 and 1
        /// </summary>
        /// <param name="context">The timer context.</param>
        /// <returns>The progress of the timer as a value between 0 and 1.</returns>
        public float GetProgress(IPersistentTimerContext context)
        {
            if (context.Accumulate)
                return 0f;
            
            double timeElapsed = GetTimeElapsed(context).TotalMilliseconds;

            double currentDuration = context.CurrentDurationSpan.TotalMilliseconds;
            
            if (currentDuration <= 0)
                return 0f;
                        
            return ((float)(timeElapsed / currentDuration)).Clamp(0f, 1f);
        }
        
        #endregion

        #region Countdown and Time elapsed

        /// <summary>
        /// Gets the time elapsed since the timer started
        /// </summary>
        /// <param name="context">The timer context.</param>
        /// <returns>The time elapsed since the timer started.</returns>
        public TimeSpan GetTimeElapsed(IPersistentTimerContext context)
        {
            return context.SavedProgress;
        }

        /// <summary>
        /// Gets the remaining time until the timer finishes
        /// </summary>
        /// <param name="context">The timer context.</param>
        /// <returns>The remaining time until the timer finishes.</returns>
        public TimeSpan GetCountdown(IPersistentTimerContext context)
        {
            return context.CurrentDurationSpan - context.SavedProgress;
        }
        
        #endregion

        #region Controls
        
        /// <summary>
        /// Resets the timer to its default state
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Reset(IPersistentTimerContext context)
        {
            context.StartTime = default(DateTime);
            
            context.EstimatedFinishTime = default(DateTime);

            context.SavedProgress = default(TimeSpan);
            
            
            context.CurrentDurationSpan = context.DefaultDurationSpan;
            
            context.SetState(ETimerState.INACTIVE);
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Start(IPersistentTimerContext context)
        {
            //ENSURE THAT CALLING START() ON A PAUSED TIMER SHOULD IGNORE A CALL INSTEAD OF RESETTING THE TIMER
        }

        /// <summary>
        /// Pauses the timer
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Pause(IPersistentTimerContext context)
        {
            //Why bother?
        }

        /// <summary>
        /// Resumes the timer
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Resume(IPersistentTimerContext context)
        {
            context.StartTime = DateTime.Now;
            
            
            context.SetState(ETimerState.STARTED);
        }
        
        /// <summary>
        /// Aborts the timer
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Abort(IPersistentTimerContext context)
        {
            context.StartTime = default(DateTime);
            
            context.EstimatedFinishTime = default(DateTime);

            context.SavedProgress = default(TimeSpan);
            
            
            context.SetState(ETimerState.INACTIVE);
        }
        
        /// <summary>
        /// Finishes the timer
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Finish(IPersistentTimerContext context)
        {
            context.EstimatedFinishTime = DateTime.Now;
            
            
            context.SetState(ETimerState.FINISHED);
            
            context.OnFinishAsPublisher.Publish((IPersistentTimer)context);
        }

        /// <summary>
        /// Executes a tick of the timer
        /// </summary>
        /// <param name="context">The timer context.</param>
        /// <param name="delta">The delta time of the tick.</param>
        public void Tick(IPersistentTimerContext context, float delta)
        {
            //Why bother?
        }
        
        #endregion
    }
}