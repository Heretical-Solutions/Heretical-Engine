using System;
using System.Collections.Generic;

using HereticalSolutions.Collections;
using HereticalSolutions.Allocations;

using HereticalSolutions.Pools.Arguments;
using HereticalSolutions.Pools.Behaviours;

namespace HereticalSolutions.Pools.Decorators
{
    /// <summary>
    /// Represents a pool that can append elements and top up the supply from another pool.
    /// </summary>
    /// <typeparam name="T">The type of elements in the pool.</typeparam>
    public class SupplyAndMergePool<T> :
        INonAllocDecoratedPool<T>,
        IAppendable<IPoolElement<T>>,
        ITopUppable<IPoolElement<T>>
    {
        private readonly INonAllocPool<T> basePool;
        
        private readonly INonAllocPool<T> supplyPool;
        
        private readonly IIndexable<IPoolElement<T>> supplyPoolAsIndexable;
        
        private readonly IFixedSizeCollection<IPoolElement<T>> supplyPoolAsFixedSizeCollection;
        
        private readonly IPushBehaviourHandler<T> pushBehaviourHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupplyAndMergePool{T}"/> class with the specified parameters.
        /// </summary>
        /// <param name="basePool">The base pool.</param>
        /// <param name="supplyPool">The supply pool.</param>
        /// <param name="supplyPoolAsIndexable">The supply pool as an indexable collection.</param>
        /// <param name="supplyPoolAsFixedSizeCollection">The supply pool as a fixed-size collection.</param>
        /// <param name="appendAllocationCommand">The allocation command for appending elements.</param>
        /// <param name="mergeDelegate">The delegate for merging the supply pool into the base pool.</param>
        /// <param name="topUpAllocationDelegate">The allocation delegate for topping up the supply pool.</param>
        public SupplyAndMergePool(
            INonAllocPool<T> basePool,
            INonAllocPool<T> supplyPool,
            IIndexable<IPoolElement<T>> supplyPoolAsIndexable,
            IFixedSizeCollection<IPoolElement<T>> supplyPoolAsFixedSizeCollection,
            AllocationCommand<IPoolElement<T>> appendAllocationCommand,
            Action<INonAllocPool<T>, INonAllocPool<T>, AllocationCommand<IPoolElement<T>>> mergeDelegate,
            Func<T> topUpAllocationDelegate)
        {
            this.basePool = basePool;
            
            this.supplyPool = supplyPool;
            
            this.supplyPoolAsIndexable = supplyPoolAsIndexable;
            
            this.supplyPoolAsFixedSizeCollection = supplyPoolAsFixedSizeCollection;
            
            this.mergeDelegate = mergeDelegate;
            
            this.topUpAllocationDelegate = topUpAllocationDelegate;
            
            AppendAllocationCommand = appendAllocationCommand;
            
            pushBehaviourHandler = new PushToDecoratedPoolBehaviour<T>(this);
        }

        #region IAppendable

        /// <summary>
        /// Gets the allocation command for appending elements.
        /// </summary>
        public AllocationCommand<IPoolElement<T>> AppendAllocationCommand { get; private set; }

        /// <summary>
        /// Appends an element to the pool.
        /// </summary>
        /// <returns>The appended element.</returns>
        public IPoolElement<T> Append()
        {
            if (!supplyPool.HasFreeSpace)
            {
                MergeSupplyIntoBase();
            }

            IPoolElement<T> result = supplyPool.Pop();

            return result;
        }

        #endregion

        #region ITopUppable

        private readonly Func<T> topUpAllocationDelegate;

        /// <summary>
        /// Tops up the specified element.
        /// </summary>
        /// <param name="element">The element to top up.</param>
        public void TopUp(IPoolElement<T> element)
        {
            element.Value = topUpAllocationDelegate.Invoke();
        }

        #endregion

        #region Merge

        private readonly Action<INonAllocPool<T>, INonAllocPool<T>, AllocationCommand<IPoolElement<T>>> mergeDelegate;

        /// <summary>
        /// Merges the supply pool into the base pool.
        /// </summary>
        private void MergeSupplyIntoBase()
        {
            mergeDelegate.Invoke(
                basePool,
                supplyPool,
                AppendAllocationCommand);
        }

        /// <summary>
        /// Tops up the supply pool and merges it into the base pool.
        /// </summary>
        private void TopUpAndMerge()
        {
            for (int i = supplyPoolAsIndexable.Count; i < supplyPoolAsFixedSizeCollection.Capacity; i++)
                TopUp(supplyPoolAsFixedSizeCollection.ElementAt(i));

            MergeSupplyIntoBase();
        }

        #endregion

        #region INonAllocDecoratedPool
        
        /// <summary>
        /// Pops an element from the pool using the specified decorator arguments.
        /// </summary>
        /// <param name="args">The decorator arguments.</param>
        /// <returns>The popped element.</returns>
        public IPoolElement<T> Pop(IPoolDecoratorArgument[] args)
        {
            #region Append from argument
            
            if (args.TryGetArgument<AppendArgument>(out var arg))
            {
                var appendee = Append();

                #region Update push behaviour

                var appendeeElementAsPushable = (IPushable<T>)appendee; 
            
                appendeeElementAsPushable.UpdatePushBehaviour(pushBehaviourHandler);

                #endregion
                
                return appendee;
            }
            
            #endregion
            
            #region Top up and merge
            
            if (!basePool.HasFreeSpace)
            {
                TopUpAndMerge();
            }
            
            #endregion

            IPoolElement<T> result = basePool.Pop();

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
        /// Pushes an element back into the pool.
        /// </summary>
        /// <param name="instance">The instance to push back.</param>
        /// <param name="decoratorsOnly">A flag indicating whether to only perform decorators.</param>
        public void Push(IPoolElement<T> instance, bool decoratorsOnly = false)
        {
            if (decoratorsOnly)
                return;

            #region Top up and merge
            
            var instanceIndex = instance.Metadata.Get<IIndexed>().Index;

            if (instanceIndex > -1
                && instanceIndex < supplyPoolAsIndexable.Count
                && supplyPoolAsIndexable[instanceIndex] == instance)
            {
                TopUpAndMerge();
            }
            
            #endregion

            basePool.Push(instance);
        }
        
        /// <summary>
        /// Gets a value indicating whether the pool has free space.
        /// </summary>
        public bool HasFreeSpace { get { return true; } }  // ¯\_(ツ)_/¯

        #endregion
    }
}