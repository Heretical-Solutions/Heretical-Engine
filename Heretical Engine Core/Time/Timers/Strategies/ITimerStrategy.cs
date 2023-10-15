namespace HereticalSolutions.Time
{
    public interface ITimerStrategy<TStrategyContext, TTimeValue>
    {
        #region Progress
        
        /// <summary>
        /// Get current progress of the timer, ranged between 0 (start) and 1 (finish)
        /// </summary>
        /// <param name="context">Timer context</param>
        /// <returns>Current timer progress</returns>
        float GetProgress(TStrategyContext context);

        #endregion

        #region Countdown and Time elapsed
        
        /// <summary>
        /// Get the elapsed time of the timer
        /// </summary>
        /// <param name="context">Timer context</param>
        /// <returns>Elapsed time</returns>
        TTimeValue GetTimeElapsed(TStrategyContext context);

        /// <summary>
        /// Get the time left for the timer
        /// </summary>
        /// <param name="context">Timer context</param>
        /// <returns>Countdown duration</returns>
        TTimeValue GetCountdown(TStrategyContext context);
        
        #endregion
        
        #region Controls
        
        /// <summary>
        /// Resets the timer. State becomes INACTIVE, time elapsed becomes zero, countdown is reset to current duration, current duration is set to default duration, start and estimated finish times are reset to default
        /// </summary>
        /// <param name="context">Timer context</param>
        void Reset(TStrategyContext context);

        /// <summary>
        /// Starts the timer. State becomes STARTED, time elapsed becomes zero, countdown is reset to current duration, start and estimated finish times are calculated
        /// </summary>
        /// <param name="context">Timer context</param>
        void Start(TStrategyContext context);

        /// <summary>
        /// Pauses the timer. State becomes PAUSED, start and estimated finish times are reset to default
        /// </summary>
        /// <param name="context">Timer context</param>
        void Pause(TStrategyContext context);

        /// <summary>
        /// Resumes the timer. State becomes STARTED, start and estimated finish times are calculated
        /// </summary>
        /// <param name="context">Timer context</param>
        void Resume(TStrategyContext context);
        
        /// <summary>
        /// Aborts the timer. State becomes INACTIVE, time elapsed becomes zero, countdown is reset to current duration, start and estimated finish times are reset to default
        /// </summary>
        /// <param name="context">Timer context</param>
        void Abort(TStrategyContext context);

        /// <summary>
        /// Finishes the timer operation prematurely. State becomes FINISHED, time elapsed becomes zero, countdown is reset to current duration, start and estimated finish times are reset to default
        /// </summary>
        /// <param name="context">Timer context</param>
        void Finish(TStrategyContext context);

        /// <summary>
        /// Perform a tick
        /// </summary>
        /// <param name="context">Timer context</param>
        /// <param name="delta">Time delta between subsequent ticks</param>
        void Tick(TStrategyContext context, float delta);
        
        #endregion
    }
}