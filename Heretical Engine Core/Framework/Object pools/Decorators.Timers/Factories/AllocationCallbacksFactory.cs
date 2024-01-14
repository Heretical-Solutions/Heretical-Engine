using HereticalSolutions.Pools.AllocationCallbacks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Represents a factory for creating timers decorators pools.
    /// </summary>
    public static partial class TimersDecoratorsPoolsFactory
    {
        #region Allocation callbacks

        public static SetRuntimeTimerCallback<T> BuildSetRuntimeTimerCallback<T>(
            string id = "Anonymous Timer",
            float defaultDuration = 0f,
            ILoggerResolver loggerResolver = null)
        {
            return new SetRuntimeTimerCallback<T>(
                id,
                defaultDuration,
                loggerResolver);
        }
        
        #endregion
    }
}