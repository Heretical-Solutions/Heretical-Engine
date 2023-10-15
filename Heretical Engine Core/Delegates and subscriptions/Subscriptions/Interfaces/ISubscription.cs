namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents a subscription to a publisher.
    /// </summary>
    public interface ISubscription
    {
        /// <summary>
        /// Gets a value indicating whether the subscription is active.
        /// </summary>
        bool Active { get; }

        ///// <summary>
        ///// Subscribes to the specified publisher.
        ///// </summary>
        ///// <typeparam name="TSubscribable">The type of the publisher.</typeparam>
        ///// <param name="publisher">The publisher to subscribe to.</param>
        //void Subscribe<TSubscribable>(TSubscribable publisher);
        
        ///// <summary>
        ///// Unsubscribes from the publisher.
        ///// </summary>
        //void Unsubscribe();
    }
}