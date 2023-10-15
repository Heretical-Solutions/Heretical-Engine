namespace HereticalSolutions.Pools.Factories
{
    public static partial class PoolsFactory
    {
        /// <summary>
        /// Builds a decorator pool that wraps an existing pool.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="innerPool">The inner pool to be wrapped.</param>
        /// <returns>A new instance of the decorator pool.</returns>
        public static DecoratorPool<T> BuildDecoratorPool<T>(IPool<T> innerPool)
        {
            return new DecoratorPool<T>(innerPool);
        }
    }
}