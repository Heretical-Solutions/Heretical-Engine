using HereticalSolutions.Pools.Arguments;

namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents a decorated pool of objects.
    /// </summary>
    /// <typeparam name="T">The type of objects in the pool.</typeparam>
    public interface IDecoratedPool<T>
    {
        /// <summary>
        /// Retrieves the next available object from the pool, applying the specified decorators.
        /// </summary>
        /// <param name="args">The array of arguments used by the decorators.</param>
        /// <returns>The next available object from the pool after applying the decorators.</returns>
        T Pop(IPoolDecoratorArgument[] args);

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        /// <param name="instance">The object to be returned to the pool.</param>
        /// <param name="decoratorsOnly">Optional. Specifies whether the object being returned belongs only to the decorators.</param>
        void Push(
            T instance,
            bool decoratorsOnly = false);
    }
}