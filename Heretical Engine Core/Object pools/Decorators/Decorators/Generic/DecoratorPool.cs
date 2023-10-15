using HereticalSolutions.Pools.Arguments;

namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents a decorator pool that decorates instances from another pool.
    /// </summary>
    /// <typeparam name="T">The type of instances being decorated.</typeparam>
    public class DecoratorPool<T> : ADecoratorPool<T>
    {
        private IPool<T> pool;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecoratorPool{T}"/> class.
        /// </summary>
        /// <param name="pool">The pool to decorate instances from.</param>
        public DecoratorPool(IPool<T> pool)
            : base(null)
        {
            this.pool = pool;
        }

        /// <summary>
        /// Retrieves an instance from the decorated pool.
        /// </summary>
        /// <param name="args">The decorator arguments.</param>
        /// <returns>The decorated instance.</returns>
        public override T Pop(IPoolDecoratorArgument[] args)
        {
            return pool.Pop();
        }

        /// <summary>
        /// Returns an instance to the decorated pool.
        /// </summary>
        /// <param name="instance">The instance to return.</param>
        /// <param name="decoratorsOnly">Whether to only return instances that were decorated.</param>
        public override void Push(
            T instance,
            bool decoratorsOnly = false)
        {
            if (!decoratorsOnly)
                pool.Push(instance);
        }
    }
}