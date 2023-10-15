namespace HereticalSolutions.Pools.Decorators
{
    /// <summary>
    /// A decorator pool that adds an ID to the items.
    /// </summary>
    /// <typeparam name="T">The type of items in the pool.</typeparam>
    public class NonAllocPoolWithID<T> : ANonAllocDecoratorPool<T>
    {
        /// <summary>
        /// Gets the ID of the pool.
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonAllocPoolWithID{T}"/> class with the specified inner pool and ID.
        /// </summary>
        /// <param name="innerPool">The inner pool to decorate.</param>
        /// <param name="id">The ID to set for the pool.</param>
        public NonAllocPoolWithID(
            INonAllocDecoratedPool<T> innerPool,
            string id)
            : base(innerPool)
        {
            ID = id;
        }
    }
}