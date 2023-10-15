namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents a container for a variant pool.
    /// </summary>
    /// <typeparam name="T">The type of objects stored in the pool.</typeparam>
    public class VariantContainer<T>
    {
        /// <summary>
        /// The chance of selecting objects from the pool.
        /// </summary>
        public float Chance;

        /// <summary>
        /// The variant pool of objects.
        /// </summary>
        public INonAllocDecoratedPool<T> Pool;
    }
}