namespace HereticalSolutions.Pools.Behaviours
{
    /// <summary>
    /// Represents a class that handles the behavior of pushing an element to a decorated pool.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the pool.</typeparam>
    public class PushToDecoratedPoolBehaviour<T> : IPushBehaviourHandler<T>
    {
        private readonly INonAllocDecoratedPool<T> pool;

        /// <summary>
        /// Initializes a new instance of the <see cref="PushToDecoratedPoolBehaviour{T}"/> class with the specified pool.
        /// </summary>
        /// <param name="pool">The decorated pool to push the elements to.</param>
        public PushToDecoratedPoolBehaviour(INonAllocDecoratedPool<T> pool)
        {
            this.pool = pool;
        }

        /// <summary>
        /// Pushes the specified element to the decorated pool.
        /// </summary>
        /// <param name="poolElement">The element to be pushed to the pool.</param>
        public void Push(IPoolElement<T> poolElement)
        {
            pool.Push(poolElement, false);
        }
    }
}