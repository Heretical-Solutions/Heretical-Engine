using System;

namespace HereticalSolutions.Time.Strategies
{
    /// <summary>
    /// Represents a timer strategy that keeps track of the time elapsed from the start and calculates the progress based on the current duration
    /// Implements the <see cref="ITimerStrategy{IPersistentTimerContext, TimeSpan}"/> interface
    /// </summary>
    public class PersistentStartedStrategy : ITimerStrategy<IPersistentTimerContext, TimeSpan>
    {
        #region Progress

        /// <summary>
        /// Retrieves the progress of the timer
        /// </summary>
        /// <param name="context">The <see cref="IPersistentTimerContext"/> object representing the timer context.</param>
        /// <returns>The progress of the timer as a value between 0 and 1. Returns 0 if the timer is set to accumulate progress.</returns>
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
        /// Retrieves the time elapsed since the timer started
        /// </summary>
        /// <param name="context">The <see cref="IPersistentTimerContext"/> object representing the timer context.</param>
        /// <returns>The time elapsed since the timer started as a <see cref="TimeSpan"/> object.</returns>
        public TimeSpan GetTimeElapsed(IPersistentTimerContext context)
        {
            return context.Accumulate
                ? ((DateTime.Now - context.EstimatedFinishTime) + context.SavedProgress)
                : ((DateTime.Now - context.StartTime) + context.SavedProgress);
        }

        /// <summary>
        /// Retrieves the remaining time until the timer finishes
        /// </summary>
        /// <param name="context">The <see cref="IPersistentTimerContext"/> object representing the timer context.</param>
        /// <returns>The remaining time until the timer finishes as a <see cref="TimeSpan"/> object.</returns>
        public TimeSpan GetCountdown(IPersistentTimerContext context)
        {
            return context.EstimatedFinishTime - DateTime.Now;
        }

        #endregion

        #region Controls

        /// <summary>
        /// Resets the timer to its initial state
        /// </summary>
        /// <param name="context">The <see cref="IPersistentTimerContext"/> object representing the timer context.</param>
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
        /// <param name="context">The <see cref="IPersistentTimerContext"/> object representing the timer context.</param>
        /// <remarks>Calling Start() on a running timer should ignore the call instead of resetting the timer.</remarks>
        public void Start(IPersistentTimerContext context)
        {
            //ENSURE THAT CALLING START() ON A RUNNING TIMER SHOULD IGNORE A CALL INSTEAD OF RESETTING THE TIMER
        }

        /// <summary>
        /// Pauses the timer
        /// </summary>
        /// <param name="context">The <see cref="IPersistentTimerContext"/> object representing the timer context.</param>
        public void Pause(IPersistentTimerContext context)
        {
            context.SavedProgress = GetTimeElapsed(context);

            context.SetState(ETimerState.PAUSED);
        }

        /// <summary>
        /// Resumes the timer
        /// </summary>
        /// <param name="context">The <see cref="IPersistentTimerContext"/> object representing the timer context.</param>
        /// <remarks>This method does not perform any action.</remarks>
        public void Resume(IPersistentTimerContext context)
        {
            //Why bother?
        }

        /// <summary>
        /// Aborts the timer, resetting it to its initial state
        /// </summary>
        /// <param name="context">The <see cref="IPersistentTimerContext"/> object representing the timer context.</param>
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
        /// <param name="context">The <see cref="IPersistentTimerContext"/> object representing the timer context.</param>
        public void Finish(IPersistentTimerContext context)
        {
            context.EstimatedFinishTime = DateTime.Now;


            context.SetState(ETimerState.FINISHED);

            context.OnFinishAsPublisher.Publish((IPersistentTimer)context);
        }

        /// <summary>
        /// Advances the timer by the specified delta time
        /// </summary>
        /// <param name="context">The <see cref="IPersistentTimerContext"/> object representing the timer context.</param>
        /// <param name="delta">The delta time in seconds.</param>
        public void Tick(IPersistentTimerContext context, float delta)
        {
            if (context.Accumulate)
            {
                var now = DateTime.Now;

                TimeSpan dateTimeDelta = now - context.EstimatedFinishTime;

                context.SavedProgress += dateTimeDelta;

                context.EstimatedFinishTime = now;

                return;
            }

            if (DateTime.Now > context.EstimatedFinishTime)
            {
                if (context.Repeat)
                {
                    if (context.FlushTimeElapsedOnRepeat)
                    {
                        context.OnFinishAsPublisher.Publish((IPersistentTimer)context);


                        context.StartTime = DateTime.Now;

                        context.EstimatedFinishTime = context.StartTime + context.CurrentDurationSpan;

                        context.SavedProgress = default(TimeSpan);


                        context.OnStartAsPublisher.Publish((IPersistentTimer)context);
                    }
                    else
                    {
                        while (DateTime.Now > context.EstimatedFinishTime)
                        {
                            context.OnFinishAsPublisher.Publish((IPersistentTimer)context);


                            context.StartTime = context.EstimatedFinishTime;

                            context.EstimatedFinishTime = context.StartTime + context.CurrentDurationSpan;

                            context.SavedProgress = default(TimeSpan);


                            context.OnStartAsPublisher.Publish((IPersistentTimer)context);
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