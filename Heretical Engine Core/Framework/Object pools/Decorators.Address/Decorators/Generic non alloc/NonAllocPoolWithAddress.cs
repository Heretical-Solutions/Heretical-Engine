using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.Pools.Arguments;
using HereticalSolutions.Pools.Behaviours;

namespace HereticalSolutions.Pools.Decorators
{
    /// <summary>
    /// Represents a decorator for a non-allocating pool with address support.
    /// </summary>
    /// <typeparam name="T">The type of object stored in the pool.</typeparam>
    public class NonAllocPoolWithAddress<T> : INonAllocDecoratedPool<T>
    {
        private readonly int level;
        private readonly IRepository<int, INonAllocDecoratedPool<T>> innerPoolsRepository;
        private readonly IPushBehaviourHandler<T> pushBehaviourHandler;

        /// <summary>
        /// Initializes a new instance of the NonAllocPoolWithAddress class.
        /// </summary>
        /// <param name="innerPoolsRepository">The repository containing the inner pools.</param>
        /// <param name="level">The level of the address.</param>
        public NonAllocPoolWithAddress(IRepository<int, INonAllocDecoratedPool<T>> innerPoolsRepository, int level)
        {
            this.innerPoolsRepository = innerPoolsRepository;
            this.level = level;
            pushBehaviourHandler = new PushToDecoratedPoolBehaviour<T>(this);
        }

        #region Pop

        /// <summary>
        /// Removes and returns an object from the pool with the specified arguments.
        /// </summary>
        /// <param name="args">The arguments used to determine which object to pop from the pool.</param>
        /// <returns>The object from the pool.</returns>
        public IPoolElement<T> Pop(IPoolDecoratorArgument[] args)
        {
            #region Validation

            if (!args.TryGetArgument<AddressArgument>(out var arg))
                throw new Exception("[NonAllocPoolWithAddress] ADDRESS ARGUMENT ABSENT");

            if (arg.AddressHashes.Length < level)
                throw new Exception($"[NonAllocPoolWithAddress] INVALID ADDRESS DEPTH. LEVEL: {{ {level} }} ADDRESS LENGTH: {{ {arg.AddressHashes.Length} }}");

            #endregion

            INonAllocDecoratedPool<T> poolByAddress = null;

            #region Pool at the end of address

            if (arg.AddressHashes.Length == level)
            {
                if (!innerPoolsRepository.TryGet(0, out poolByAddress))
                    throw new Exception($"[NonAllocPoolWithAddress] NO POOL DETECTED AT THE END OF ADDRESS. LEVEL: {{ {level} }}");

                var endOfAddressResult = poolByAddress.Pop(args);

                // Update element data
                var endOfAddressElementAsPushable = (IPushable<T>)endOfAddressResult;
                endOfAddressElementAsPushable.UpdatePushBehaviour(pushBehaviourHandler);

                return endOfAddressResult;
            }

            #endregion

            #region Pool at current level of address

            int currentAddressHash = arg.AddressHashes[level];

            if (!innerPoolsRepository.TryGet(currentAddressHash, out poolByAddress))
                throw new Exception($"[NonAllocPoolWithAddress] INVALID ADDRESS {{ {arg.FullAddress} }} ADDRESS HASH: {{ {currentAddressHash} }} LEVEL: {{ {level} }}");

            var result = poolByAddress.Pop(args);

            // Update element data
            var elementAsPushable = (IPushable<T>)result;
            elementAsPushable.UpdatePushBehaviour(pushBehaviourHandler);

            return result;

            #endregion
        }

        #endregion

        #region Push

        /// <summary>
        /// Pushes an object back into the pool.
        /// </summary>
        /// <param name="instance">The object to push back into the pool.</param>
        /// <param name="decoratorsOnly">
        ///     If set to true, only the decorators of the object will be pushed back into the pool.
        ///     Otherwise, the entire object will be pushed back.
        /// </param>
        public void Push(IPoolElement<T> instance, bool decoratorsOnly = false)
        {
            if (!instance.Metadata.Has<IContainsAddress>())
                throw new Exception("[NonAllocPoolWithAddress] INVALID INSTANCE");

            INonAllocDecoratedPool<T> pool = null;

            var addressHashes = instance.Metadata.Get<IContainsAddress>().AddressHashes;

            if (addressHashes.Length == level)
            {
                if (!innerPoolsRepository.TryGet(0, out pool))
                    throw new Exception($"[NonAllocPoolWithAddress] NO POOL DETECTED AT ADDRESS MAX. DEPTH. LEVEL: {{ {level} }}");

                pool.Push(instance, decoratorsOnly);
                return;
            }

            int currentAddressHash = addressHashes[level];

            if (!innerPoolsRepository.TryGet(currentAddressHash, out pool))
                throw new Exception($"[NonAllocPoolWithAddress] INVALID ADDRESS {{ {currentAddressHash} }}");

            pool.Push(instance, decoratorsOnly);
        }

        #endregion
    }
}