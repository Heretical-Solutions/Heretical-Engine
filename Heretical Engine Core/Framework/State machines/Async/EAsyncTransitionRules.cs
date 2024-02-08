namespace HereticalSolutions.StateMachines
{
    /// <summary>
    /// An enumeration of asynchronous transition rules
    /// </summary>
    public enum EAsyncTransitionRules
    {
        /// <summary>
        /// Transition sequentially: unload then load
        /// </summary>
        TRANSIT_SEQUENTIALLY_UNLOAD_THEN_LOAD,

        /// <summary>
        /// Transition sequentially: load then unload
        /// </summary>
        TRANSIT_SEQUENTIALLY_LOAD_THEN_UNLOAD,

        /// <summary>
        /// Transition simultaneously
        /// </summary>
        TRANSIT_SIMULTANEOUSLY
    }
}