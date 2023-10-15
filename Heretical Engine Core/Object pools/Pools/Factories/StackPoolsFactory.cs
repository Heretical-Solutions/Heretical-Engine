using System;
using System.Collections.Generic;

using HereticalSolutions.Collections;
using HereticalSolutions.Allocations;

using HereticalSolutions.Pools.Generic;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Represents a factory for creating various types of object pools.
    /// </summary>
    public static partial class PoolsFactory
    {
        #region Stack pool

        /// <summary>
        /// Builds a stack pool with the specified initial allocation command and additional allocation command.
        /// </summary>
        /// <typeparam name="T">The type of objects to be stored in the pool.</typeparam>
        /// <param name="initialAllocationCommand">The initial allocation command for the pool.</param>
        /// <param name="additionalAllocationCommand">The additional allocation command for the pool.</param>
        /// <returns>A stack pool with the specified allocation commands.</returns>
        public static StackPool<T> BuildStackPool<T>(
            AllocationCommand<T> initialAllocationCommand,
            AllocationCommand<T> additionalAllocationCommand)
        {
            var stack = new Stack<T>();

            PerformInitialAllocation<T>(stack, initialAllocationCommand);

            return new StackPool<T>(
                stack,
                ResizeStackPool,
                additionalAllocationCommand);
        }

        /// <summary>
        /// Performs the initial allocation of objects in the stack pool.
        /// </summary>
        /// <typeparam name="T">The type of objects in the stack pool.</typeparam>
        /// <param name="stack">The stack in which objects are allocated.</param>
        /// <param name="initialAllocationCommand">The initial allocation command for the stack pool.</param>
        private static void PerformInitialAllocation<T>(
            Stack<T> stack,
            AllocationCommand<T> initialAllocationCommand)
        {
            int initialAmount = -1;

            switch (initialAllocationCommand.Descriptor.Rule)
            {
                case EAllocationAmountRule.ZERO:
                    initialAmount = 0;
                    break;

                case EAllocationAmountRule.ADD_ONE:
                    initialAmount = 1;
                    break;

                case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
                    initialAmount = initialAllocationCommand.Descriptor.Amount;
                    break;

                default:
                    throw new Exception($"[CollectionFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Descriptor.Rule.ToString()}");
            }

            for (int i = 0; i < initialAmount; i++)
                stack.Push(
                    initialAllocationCommand.AllocationDelegate());
        }

        /// <summary>
        /// Resizes the stack pool by allocating additional objects.
        /// </summary>
        /// <typeparam name="T">The type of objects in the stack pool.</typeparam>
        /// <param name="pool">The stack pool to be resized.</param>
        public static void ResizeStackPool<T>(
            StackPool<T> pool)
        {
            var stack = ((IModifiable<Stack<T>>)pool).Contents;

            var allocationCommand = ((IResizable<T>)pool).ResizeAllocationCommand;

            int addedCapacity = -1;

            switch (allocationCommand.Descriptor.Rule)
            {
                case EAllocationAmountRule.ADD_ONE:
                    addedCapacity = 1;
                    break;

                case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
                    addedCapacity = allocationCommand.Descriptor.Amount;
                    break;

                default:
                    throw new Exception($"[CollectionFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Descriptor.Rule.ToString()}");
            }

            for (int i = 0; i < addedCapacity; i++)
                stack.Push(
                    allocationCommand.AllocationDelegate());
        }

        #endregion
    }
}