namespace HereticalSolutions.Pools.Behaviours
{
    /// <summary>
    /// Represents a behavior that pushes a pool element to an INonAllocPool
    /// </summary>
    /// <typeparam name="T">The type of the pool element.</typeparam>
    public class PushToINonAllocPoolBehaviour<T> : IPushBehaviourHandler<T>
    {
        private readonly INonAllocPool<T> pool;

        /// <summary>
        /// Initializes a new instance of the PushToINonAllocPoolBehaviour class with the specified INonAllocPool
        /// </summary>
        /// <param name="pool">The INonAllocPool where the pool elements will be pushed to.</param>
        public PushToINonAllocPoolBehaviour(INonAllocPool<T> pool)
        {
            this.pool = pool;
        }

        /// <summary>
        /// Pushes a pool element to the INonAllocPool
        /// </summary>
        /// <param name="poolElement">The pool element to be pushed.</param>
        public void Push(IPoolElement<T> poolElement)
        {
            pool.Push(poolElement);
        }
    }
}