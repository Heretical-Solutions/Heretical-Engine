using HereticalSolutions.Pools.Arguments;
using HereticalSolutions.Pools.Behaviours;

namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents an abstract class for a non-allocating decorator pool.
    /// </summary>
    /// <typeparam name="T">The type of objects stored in the pool.</typeparam>
    public abstract class ANonAllocDecoratorPool<T>
        : INonAllocDecoratedPool<T>
    {
        protected INonAllocDecoratedPool<T> innerPool;

        private readonly IPushBehaviourHandler<T> pushBehaviourHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ANonAllocDecoratorPool{T}"/> class.
        /// </summary>
        /// <param name="innerPool">The inner pool to be decorated.</param>
        public ANonAllocDecoratorPool(
            INonAllocDecoratedPool<T> innerPool)
        {
            this.innerPool = innerPool;

            pushBehaviourHandler = new PushToDecoratedPoolBehaviour<T>(this);
        }

        #region Pop

        /// <summary>
        /// Retrieves an object from the pool, with the specified decorator arguments applied.
        /// </summary>
        /// <param name="args">The arguments for applying decorators.</param>
        /// <returns>The pooled object.</returns>
        public virtual IPoolElement<T> Pop(IPoolDecoratorArgument[] args)
        {
            OnBeforePop(args);

            IPoolElement<T> result = innerPool.Pop(args);

            #region Update push behaviour

            var elementAsPushable = (IPushable<T>)result;

            elementAsPushable.UpdatePushBehaviour(pushBehaviourHandler);

            #endregion

            OnAfterPop(result, args);

            return result;
        }

        /// <summary>
        /// Invoked before an object is retrieved from the pool.
        /// </summary>
        /// <param name="args">The arguments for applying decorators.</param>
        protected virtual void OnBeforePop(IPoolDecoratorArgument[] args)
        {
        }

        /// <summary>
        /// Invoked after an object is retrieved from the pool.
        /// </summary>
        /// <param name="instance">The retrieved object.</param>
        /// <param name="args">The arguments for applying decorators.</param>
        protected virtual void OnAfterPop(
            IPoolElement<T> instance,
            IPoolDecoratorArgument[] args)
        {
        }

        #endregion

        #region Push

        /// <summary>
        /// Returns an object back to the pool, with optional consideration for decorators only.
        /// </summary>
        /// <param name="instance">The object to be returned to the pool.</param>
        /// <param name="decoratorsOnly">If true, only decorators will be considered. If false, decorators and the object itself will be considered.</param>
        public virtual void Push(
            IPoolElement<T> instance,
            bool decoratorsOnly = false)
        {
            OnBeforePush(instance);

            innerPool.Push(
                instance,
                decoratorsOnly);

            OnAfterPush(instance);
        }

        /// <summary>
        /// Invoked before an object is returned to the pool.
        /// </summary>
        /// <param name="instance">The object to be returned to the pool.</param>
        protected virtual void OnBeforePush(IPoolElement<T> instance)
        {
        }

        /// <summary>
        /// Invoked after an object is returned to the pool.
        /// </summary>
        /// <param name="instance">The object to be returned to the pool.</param>
        protected virtual void OnAfterPush(IPoolElement<T> instance)
        {
        }

        #endregion
    }
}