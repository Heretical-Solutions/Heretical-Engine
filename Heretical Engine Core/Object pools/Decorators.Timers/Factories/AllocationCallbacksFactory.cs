using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Represents a factory for creating timers decorators pools.
    /// </summary>
    public static partial class TimersDecoratorsPoolsFactory
    {
        #region Allocation callbacks

        /// <summary>
        /// Builds a set runtime timer callback.
        /// </summary>
        /// <typeparam name="T">The type of the timer.</typeparam>
        /// <param name="id">The ID of the timer.</param>
        /// <param name="defaultDuration">The default duration of the timer.</param>
        /// <returns>A new instance of the <see cref="SetRuntimeTimerCallback{T}"/> class.</returns>
        public static SetRuntimeTimerCallback<T> BuildSetRuntimeTimerCallback<T>(
            string id = "Anonymous Timer",
            float defaultDuration = 0f)
        {
            return new SetRuntimeTimerCallback<T>(id, defaultDuration);
        }
        
        #endregion
    }
}