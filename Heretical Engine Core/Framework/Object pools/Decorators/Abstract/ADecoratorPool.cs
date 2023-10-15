using HereticalSolutions.Pools.Arguments;

namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents an abstract base class for decorator pools.
    /// </summary>
    /// <typeparam name="T">The type of objects stored in the pool.</typeparam>
    public abstract class ADecoratorPool<T> : IDecoratedPool<T>
    {
        protected IDecoratedPool<T> innerPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="ADecoratorPool{T}"/> class with the specified inner pool.
        /// </summary>
        /// <param name="innerPool">The inner pool to decorate.</param>
        public ADecoratorPool(IDecoratedPool<T> innerPool)
        {
            this.innerPool = innerPool;
        }

        #region Pop

        /// <summary>
        /// Pops an object from the pool.
        /// </summary>
        /// <param name="args">The arguments provided for object decoration.</param>
        /// <returns>The popped object from the pool.</returns>
        public virtual T Pop(IPoolDecoratorArgument[] args)
        {
            OnBeforePop(args);

            T result = innerPool.Pop(args);

            OnAfterPop(result, args);

            return result;
        }

        /// <summary>
        /// Called before an object is popped from the pool.
        /// </summary>
        /// <param name="args">The arguments provided for object decoration.</param>
        protected virtual void OnBeforePop(IPoolDecoratorArgument[] args)
        {
        }

        /// <summary>
        /// Called after an object is popped from the pool.
        /// </summary>
        /// <param name="instance">The popped object from the pool.</param>
        /// <param name="args">The arguments provided for object decoration.</param>
        protected virtual void OnAfterPop(
            T instance,
            IPoolDecoratorArgument[] args)
        {
        }

        #endregion

        #region Push

        /// <summary>
        /// Pushes an object back into the pool.
        /// </summary>
        /// <param name="instance">The object to push back into the pool.</param>
        /// <param name="decoratorsOnly">A boolean value indicating whether only decorators should be pushed back into the pool.</param>
        public virtual void Push(
            T instance,
            bool decoratorsOnly = false)
        {
            OnBeforePush(instance);

            innerPool.Push(
                instance,
                decoratorsOnly);

            OnAfterPush(instance);
        }

        /// <summary>
        /// Called before an object is pushed back into the pool.
        /// </summary>
        /// <param name="instance">The object to push back into the pool.</param>
        protected virtual void OnBeforePush(T instance)
        {
        }

        /// <summary>
        /// Called after an object is pushed back into the pool.
        /// </summary>
        /// <param name="instance">The object pushed back into the pool.</param>
        protected virtual void OnAfterPush(T instance)
        {
        }

        #endregion
    }
}