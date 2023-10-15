namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents an interface for pushable objects in a pool.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    public interface IPushable<T>
    {
        /// <summary>
        /// Gets or sets the status of the pushable object.
        /// </summary>
        EPoolElementStatus Status { set; }

        /// <summary>
        /// Updates the push behavior of the pushable object using the specified push behavior handler.
        /// </summary>
        /// <param name="pushBehaviourHandler">The push behavior handler to use for updating the push behavior.</param>
        void UpdatePushBehaviour(IPushBehaviourHandler<T> pushBehaviourHandler);
    }
}