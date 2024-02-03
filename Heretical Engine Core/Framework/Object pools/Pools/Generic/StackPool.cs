using HereticalSolutions.Collections;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Generic
{
    /// <summary>
    /// Represents a generic stack pool.
    /// </summary>
    /// <typeparam name="T">The type of elements in the pool.</typeparam>
    public class StackPool<T> 
        : IPool<T>,
          IResizable<T>,
          IModifiable<Stack<T>>,
          ICountUpdateable,
          ICleanUppable,
          IDisposable
    {
        private readonly ILogger logger;

        //Not readonly because IModifiable's UpdateContents method replaces one pool with another
        private Stack<T> pool;

        /// <summary>
        /// Initializes a new instance of the <see cref="StackPool{T}"/> class.
        /// </summary>
        /// <param name="pool">The stack to be used as the pool.</param>
        /// <param name="resizeDelegate">The delegate used for resizing the pool.</param>
        /// <param name="allocationCommand">The allocation command used for resizing the pool.</param>
        public StackPool(
            Stack<T> pool,
            Action<StackPool<T>> resizeDelegate,
            AllocationCommand<T> allocationCommand,
            ILogger logger = null)
        {
            this.pool = pool;

            this.resizeDelegate = resizeDelegate;

            this.logger = logger;

            ResizeAllocationCommand = allocationCommand;
        }
        
        #region IModifiable

        /// <summary>
        /// Gets the contents of the pool.
        /// </summary>
        public Stack<T> Contents { get => pool; }

        /// <summary>
        /// Updates the contents of the pool with the specified stack.
        /// </summary>
        /// <param name="newContents">The new stack to use as the pool's contents.</param>
        public void UpdateContents(Stack<T> newContents)
        {
            pool = newContents;
        }

        /// <summary>
        /// Updates the count of the pool.
        /// </summary>
        /// <param name="newCount">The new count for the pool.</param>
        public void UpdateCount(int newCount)
        {
            throw new Exception(
                logger.TryFormat<StackPool<T>>(
                    "CANNOT UPDATE COUNT OF STACK"));
        }

        #endregion
        
        #region IResizable

        /// <summary>
        /// Gets the allocation command used for resizing the pool.
        /// </summary>
        public AllocationCommand<T> ResizeAllocationCommand { get; private set; }

        private readonly Action<StackPool<T>> resizeDelegate;

        /// <summary>
        /// Resizes the pool using the resize delegate.
        /// </summary>
        public void Resize()
        {
            resizeDelegate(this);
        }

        #endregion

        #region IPool

        /// <summary>
        /// Removes and returns the top element from the pool.
        /// </summary>
        /// <returns>The top element from the pool.</returns>
        public T Pop()
        {
            T result = default(T);

            if (pool.Count != 0)
            {
                result = pool.Pop();
            }
            else
            {
                resizeDelegate(this);

                result = pool.Pop();
            }
            
            return result;
        }

        /// <summary>
        /// Pushes an element into the pool.
        /// </summary>
        /// <param name="instance">The element to push into the pool.</param>
        public void Push(T instance)
        {
            pool.Push(instance);
        }
        
        /// <summary>
        /// Gets a value indicating whether the pool has free space.
        /// </summary>
        public bool HasFreeSpace { get { return true; } } // ¯\_(ツ)_/¯

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            foreach (var item in pool)
                if (item is ICleanUppable)
                    (item as ICleanUppable).Cleanup();

            pool.Clear();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            foreach (var item in pool)
                if (item is IDisposable)
                    (item as IDisposable).Dispose();

            pool.Clear();
        }

        #endregion
    }
}