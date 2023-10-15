namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Represents a chain of decorator pools.
    /// </summary>
    /// <typeparam name="T">The type of objects in the pool.</typeparam>
    public class DecoratorPoolChain<T>
    {
        /// <summary>
        /// Gets or sets the top wrapper of the decorator pool chain.
        /// </summary>
        public IDecoratedPool<T> TopWrapper { get; private set; }

        /// <summary>
        /// Adds a new wrapper to the decorator pool chain.
        /// </summary>
        /// <param name="newWrapper">The new wrapper to add.</param>
        /// <returns>The current decorator pool chain.</returns>
        public DecoratorPoolChain<T> Add(IDecoratedPool<T> newWrapper)
        {
            TopWrapper = newWrapper;

            return this;
        }

        /// <summary>
        /// Adds a new wrapper to the decorator pool chain and returns the added wrapper.
        /// </summary>
        /// <param name="newWrapper">The new wrapper to add.</param>
        /// <param name="wrapperOut">The added wrapper.</param>
        /// <returns>The current decorator pool chain.</returns>
        public DecoratorPoolChain<T> Add(
            IDecoratedPool<T> newWrapper,
            out IDecoratedPool<T> wrapperOut)
        {
            TopWrapper = newWrapper;

            wrapperOut = newWrapper;

            return this;
        }
    }
}