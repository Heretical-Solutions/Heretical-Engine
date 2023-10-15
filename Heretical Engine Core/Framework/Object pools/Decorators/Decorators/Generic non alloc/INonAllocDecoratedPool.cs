using HereticalSolutions.Pools.Arguments;

namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents a generic pool that supports non-allocating decoration of pooled objects.
    /// </summary>
    /// <typeparam name="T">The type of the pooled objects.</typeparam>
    public interface INonAllocDecoratedPool<T>
    {
        /// <summary>
        /// Retrieves a decorated instance from the pool.
        /// </summary>
        /// <param name="args">The array of arguments passed to the pool decorator.</param>
        /// <returns>The decorated instance retrieved from the pool.</returns>
        IPoolElement<T> Pop(IPoolDecoratorArgument[] args);

        /// <summary>
        /// Returns a decorated instance to the pool.
        /// </summary>
        /// <param name="instance">The decorated instance to be returned.</param>
        /// <param name="decoratorsOnly">A flag indicating if the instance consists of decorators only.</param>
        void Push(
            IPoolElement<T> instance,
            bool decoratorsOnly = false);
    }
}