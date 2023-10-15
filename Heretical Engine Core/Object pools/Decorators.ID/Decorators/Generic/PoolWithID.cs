namespace HereticalSolutions.Pools.Decorators
{
    /// <summary>
    /// Represents a decorator pool with an ID.
    /// </summary>
    /// <typeparam name="T">The type of objects in the pool.</typeparam>
    public class PoolWithID<T> : ADecoratorPool<T>
    {
        /// <summary>
        /// Gets the ID of the pool.
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolWithID{T}"/> class.
        /// </summary>
        /// <param name="innerPool">The inner pool to decorate.</param>
        /// <param name="id">The ID of the pool.</param>
        public PoolWithID(
            IDecoratedPool<T> innerPool,
            string id)
            : base(innerPool)
        {
            ID = id;
        }
    }
}