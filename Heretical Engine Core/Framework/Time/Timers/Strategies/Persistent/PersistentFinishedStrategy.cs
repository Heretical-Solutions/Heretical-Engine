using System;

namespace HereticalSolutions.Time.Strategies
{
    /// <summary>
    /// Represents a strategy for a persistent timer
    /// </summary>
    public class PersistentFinishedStrategy : ITimerStrategy<IPersistentTimerContext, TimeSpan>
    {
        #region Progress

        /// <summary>
        /// Gets the progress of the timer
        /// </summary>
        /// <param name="context">The timer context.</param>
        /// <returns>The progress of the timer as a float value between 0 and 1.</returns>
        /// <remarks>
        /// This method is used to determine how much progress has been made towards the completion of the timer
        /// If the timer was finished prematurely by a <see cref="Finish"/> call rather than the timer actually running out, we might be curious how much progress was made so far
        /// </remarks>
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
        /// <returns>The time elapsed as a <see cref="TimeSpan"/>.</returns>
        public TimeSpan GetTimeElapsed(IPersistentTimerContext context)
        {
            return context.SavedProgress;
        }

        /// <summary>
        /// Gets the countdown time until the timer reaches its target duration
        /// </summary>
        /// <param name="context">The timer context.</param>
        /// <returns>The countdown time as a <see cref="TimeSpan"/>.</returns>
        public TimeSpan GetCountdown(IPersistentTimerContext context)
        {
            return context.CurrentDurationSpan - context.SavedProgress;
        }

        #endregion

        #region Controls

        /// <summary>
        /// Resets the timer to its initial state
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
            context.StartTime = DateTime.Now;

            context.EstimatedFinishTime = context.StartTime + context.CurrentDurationSpan;

            context.SavedProgress = default(TimeSpan);


            context.SetState(ETimerState.STARTED);

            context.OnStartAsPublisher.Publish((IPersistentTimer)context);
        }

        /// <summary>
        /// Pauses the timer
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Pause(IPersistentTimerContext context)
        {
            // Why bother?
        }

        /// <summary>
        /// Resumes the timer
        /// </summary>
        /// <param name="context">The timer context.</param>
        public void Resume(IPersistentTimerContext context)
        {
            // Why bother?
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
            // Why bother?
        }

        /// <summary>
        /// Advances the timer by one frame
        /// </summary>
        /// <param name="context">The timer context.</param>
        /// <param name="delta">The time since the last frame in seconds.</param>
        public void Tick(IPersistentTimerContext context, float delta)
        {
            // Why bother?
        }

        #endregion
    }
}