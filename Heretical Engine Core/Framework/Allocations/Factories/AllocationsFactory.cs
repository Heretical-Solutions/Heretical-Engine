using System;

namespace HereticalSolutions.Allocations.Factories
{
    /// <summary>
    /// A factory class for creating various types of allocations.
    /// </summary>
    public static class AllocationsFactory
    {
        /// <summary>
        /// Creates a null allocation delegate.
        /// </summary>
        /// <typeparam name="T">The type of the allocation.</typeparam>
        /// <returns>The null allocation.</returns>
        public static T NullAllocationDelegate<T>()
        {
            return default(T);
        }

        /// <summary>
        /// Creates an allocation delegate with a given instance.
        /// </summary>
        /// <typeparam name="T">The type of the allocation.</typeparam>
        /// <param name="instance">The instance to be allocated.</param>
        /// <returns>The allocation with the given instance.</returns>
        public static T InstanceAllocationDelegate<T>(T instance)
        {
            return instance;
        }

        /// <summary>
        /// Creates an allocation delegate with a given function.
        /// </summary>
        /// <typeparam name="T">The type of the allocation.</typeparam>
        /// <param name="allocationDelegate">The function to be used as an allocation delegate.</param>
        /// <returns>The allocation created by the function.</returns>
        public static T FuncAllocationDelegate<T>(Func<T> allocationDelegate)
        {
            return (allocationDelegate != null)
                ? allocationDelegate.Invoke()
                : default(T);
        }
		
        /// <summary>
        /// Creates an allocation delegate with a given function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TValue">The type of the allocation.</typeparam>
        /// <param name="allocationDelegate">The function to be used as an allocation delegate.</param>
        /// <returns>The allocation created by the function.</returns>
        public static TResult FuncAllocationDelegate<TResult, TValue>(Func<TValue> allocationDelegate) where TValue : TResult
        {
            TValue result = (allocationDelegate != null)
                ? allocationDelegate.Invoke()
                : default(TValue);

            return (TResult)result;
        }

        /// <summary>
        /// Creates an allocation delegate using the activator.
        /// </summary>
        /// <typeparam name="T">The type of the allocation.</typeparam>
        /// <returns>The allocation created using the activator.</returns>
        public static T ActivatorAllocationDelegate<T>()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
		
        /// <summary>
        /// Creates an allocation delegate using the activator.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TValue">The type of the allocation.</typeparam>
        /// <returns>The allocation created using the activator.</returns>
        public static TResult ActivatorAllocationDelegate<TResult, TValue>()
        {
            return (TResult)Activator.CreateInstance(typeof(TValue));
        }

        /// <summary>
        /// Creates an allocation delegate using the activator.
        /// </summary>
        /// <param name="valueType">The type of the allocation.</param>
        /// <returns>The allocation created using the activator.</returns>
        public static object ActivatorAllocationDelegate(Type valueType)
        {
            return Activator.CreateInstance(valueType);
        }

        public static T ActivatorAllocationDelegate<T>(object[] arguments)
        {
            return (T)Activator.CreateInstance(typeof(T), arguments);
        }
        
        public static TResult ActivatorAllocationDelegate<TResult, TValue>(object[] arguments)
        {
            return (TResult)Activator.CreateInstance(typeof(TValue), arguments);
        }
        
        public static object ActivatorAllocationDelegate(Type valueType, object[] arguments)
        {
            return Activator.CreateInstance(valueType, arguments);
        }
    }
}