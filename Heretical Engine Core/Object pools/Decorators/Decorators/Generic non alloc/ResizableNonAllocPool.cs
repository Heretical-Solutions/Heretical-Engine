using System;
using System.Collections.Generic;

using HereticalSolutions.Collections;
using HereticalSolutions.Allocations;

using HereticalSolutions.Pools.Arguments;
using HereticalSolutions.Pools.Behaviours;

namespace HereticalSolutions.Pools.Decorators
{
    /// <summary>
    /// A resizable non-allocating pool that can be decorated.
    /// </summary>
    /// <typeparam name="T">The type of objects stored in the pool.</typeparam>
    public class ResizableNonAllocPool<T>
        : INonAllocDecoratedPool<T>,
          IResizable<IPoolElement<T>>,
          IModifiable<INonAllocPool<T>>,
          ITopUppable<IPoolElement<T>>,
          ICountUpdateable
    {
        private INonAllocPool<T> contents;

        private readonly ICountUpdateable contentsAsCountUpdateable;

        private readonly IPushBehaviourHandler<T> pushBehaviourHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizableNonAllocPool{T}"/> class.
        /// </summary>
        /// <param name="contents">The initial contents of the pool.</param>
        /// <param name="contentsAsCountUpdateable">The initial contents of the pool.</param>
        /// <param name="resizeDelegate">The method to invoke when resizing the pool.</param>
        /// <param name="resizeAllocationCommand">The allocation command used for resizing.</param>
        /// <param name="topUpAllocationDelegate">The method to invoke when topping up an element of the pool.</param>
        public ResizableNonAllocPool(
            INonAllocPool<T> contents,
            ICountUpdateable contentsAsCountUpdateable,
            Action<ResizableNonAllocPool<T>> resizeDelegate,
            AllocationCommand<IPoolElement<T>> resizeAllocationCommand,
            Func<T> topUpAllocationDelegate)
        {
            this.contents = contents;

            this.contentsAsCountUpdateable = contentsAsCountUpdateable;

            this.resizeDelegate = resizeDelegate;

            this.topUpAllocationDelegate = topUpAllocationDelegate;

            ResizeAllocationCommand = resizeAllocationCommand;

            pushBehaviourHandler = new PushToDecoratedPoolBehaviour<T>(this);
        }

        #region IModifiable

        /// <summary>
        /// Gets the contents of the pool.
        /// </summary>
        public INonAllocPool<T> Contents { get => contents; }

        /// <summary>
        /// Updates the contents of the pool with a new pool.
        /// </summary>
        /// <param name="newContents">The new pool contents.</param>
        public void UpdateContents(INonAllocPool<T> newContents)
        {
            contents = newContents;
        }

        /// <summary>
        /// Updates the count of elements in the pool.
        /// </summary>
        /// <param name="newCount">The new count of elements.</param>
        public void UpdateCount(int newCount)
        {
            contentsAsCountUpdateable.UpdateCount(newCount);
        }

        #endregion

        #region IResizable

        /// <summary>
        /// Gets or sets the allocation command used for resizing the pool.
        /// </summary>
        public AllocationCommand<IPoolElement<T>> ResizeAllocationCommand { get; private set; }

        protected Action<ResizableNonAllocPool<T>> resizeDelegate;

        /// <summary>
        /// Resizes the pool.
        /// </summary>
        public void Resize()
        {
            resizeDelegate(this);
        }

        #endregion

        #region ITopUppable

        private readonly Func<T> topUpAllocationDelegate;

        /// <summary>
        /// Toppes up an element of the pool with a new value.
        /// </summary>
        /// <param name="element">The element to top up.</param>
        public void TopUp(IPoolElement<T> element)
        {
            element.Value = topUpAllocationDelegate.Invoke();
        }

        #endregion

        #region INonAllocDecoratedPool

        /// <summary>
        /// Pops an element from the pool.
        /// </summary>
        /// <param name="args">An array of pool decorator arguments.</param>
        /// <returns>The popped element.</returns>
        public IPoolElement<T> Pop(IPoolDecoratorArgument[] args)
        {
            #region Resize

            if (!contents.HasFreeSpace)
            {
                resizeDelegate(this);
            }

            #endregion

            IPoolElement<T> result = contents.Pop();

            #region Top up

            if (EqualityComparer<T>.Default.Equals(result.Value, default(T)))
            {
                TopUp(result);
            }

            #endregion

            #region Update push behaviour

            var elementAsPushable = (IPushable<T>)result;

            elementAsPushable.UpdatePushBehaviour(pushBehaviourHandler);

            #endregion

            return result;
        }

        /// <summary>
        /// Pushes an element into the pool.
        /// </summary>
        /// <param name="instance">The element to push.</param>
        /// <param name="decoratorsOnly">A flag indicating if only decorators should be pushed.</param>
        public void Push(
            IPoolElement<T> instance,
            bool decoratorsOnly = false)
        {
            if (!decoratorsOnly)
                contents.Push(instance);
        }

        /// <summary>
        /// Gets a flag indicating if the pool has free space.
        /// </summary>
        public bool HasFreeSpace { get { return contents.HasFreeSpace; } }

        #endregion
    }
}