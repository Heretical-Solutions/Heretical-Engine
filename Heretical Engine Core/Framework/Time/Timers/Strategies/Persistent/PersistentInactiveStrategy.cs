using System;

namespace HereticalSolutions.Time.Strategies
{
    ///<summary>
    /// Represents a timer strategy that handles persistent timers. 
    ///</summary>
    public class PersistentInactiveStrategy : ITimerStrategy<IPersistentTimerContext, TimeSpan>
    {
        #region Progress
        
        ///<summary>
        /// Gets the progress of the timer represented as a floating point value ranging between 0 and 1. 
        ///</summary>
        ///<param name="context">The persistent timer context.</param>
        ///<returns>The progress of the timer.</returns>
        public float GetProgress(IPersistentTimerContext context)
        {
            return 0f;
        }
        
        #endregion
        
        #region Countdown and Time elapsed
        
        ///<summary>
        /// Gets the time elapsed since the timer started. 
        ///</summary>
        ///<param name="context">The persistent timer context.</param>
        ///<returns>The time elapsed since the timer started.</returns>
        public TimeSpan GetTimeElapsed(IPersistentTimerContext context)
        {
            return default(TimeSpan);
        }
        
        ///<summary>
        /// Gets the remaining time until the timer finishes. 
        ///</summary>
        ///<param name="context">The persistent timer context.</param>
        ///<returns>The remaining time until the timer finishes.</returns>
        public TimeSpan GetCountdown(IPersistentTimerContext context)
        {
            return default(TimeSpan);
        }
        
        #endregion
        
        #region Controls
        
        ///<summary>
        /// Resets the persistent timer to its default state
        ///</summary>
        ///<param name="context">The persistent timer context.</param>
        public void Reset(IPersistentTimerContext context)
        {
            context.StartTime = default(DateTime);
            
            context.EstimatedFinishTime = default(DateTime);

            context.SavedProgress = default(TimeSpan);
            
            
            context.CurrentDurationSpan = context.DefaultDurationSpan;
        }
        
        ///<summary>
        /// Starts the persistent timer
        ///</summary>
        ///<param name="context">The persistent timer context.</param>
        public void Start(IPersistentTimerContext context)
        {
            context.StartTime = DateTime.Now;

            context.EstimatedFinishTime = context.StartTime + context.CurrentDurationSpan;
            
            context.SavedProgress = default(TimeSpan);
            
            
            context.SetState(ETimerState.STARTED);
            
            context.OnStartAsPublisher.Publish((IPersistentTimer)context);
        }
        
        ///<summary>
        /// Pauses the persistent timer
        ///</summary>
        ///<param name="context">The persistent timer context.</param>
        public void Pause(IPersistentTimerContext context)
        {
            //Why bother?
        }
        
        ///<summary>
        /// Resumes the persistent timer
        ///</summary>
        ///<param name="context">The persistent timer context.</param>
        public void Resume(IPersistentTimerContext context)
        {
            //Why bother?
        }
        
        ///<summary>
        /// Aborts the persistent timer, resetting it to its default state
        ///</summary>
        ///<param name="context">The persistent timer context.</param>
        public void Abort(IPersistentTimerContext context)
        {
            context.StartTime = default(DateTime);
            
            context.EstimatedFinishTime = default(DateTime);
            
            context.SavedProgress = default(TimeSpan);
        }
        
        ///<summary>
        /// Finishes the persistent timer
        ///</summary>
        ///<param name="context">The persistent timer context.</param>
        public void Finish(IPersistentTimerContext context)
        {
            //ENSURE WHETHER CALLING FINISH() ON INACTIVE TIMER SHOULD NOT BE CALLING A CALLBACK
        }
        
        ///<summary>
        /// Updates the state of the persistent timer
        ///</summary>
        ///<param name="context">The persistent timer context.</param>
        ///<param name="delta">The time passed since the last update.</param>
        public void Tick(IPersistentTimerContext context, float delta)
        {
            //Why bother?
        }
        
        #endregion
    }
}