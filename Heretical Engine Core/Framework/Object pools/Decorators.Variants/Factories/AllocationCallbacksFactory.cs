using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Represents a factory for creating set variant callbacks for variant decorators pools.
    /// </summary>
    public static partial class VariantsDecoratorsPoolsFactory
    {
        #region Allocation callbacks

        /// <summary>
        /// Builds a set variant callback for the specified type with an optional variant index.
        /// </summary>
        /// <typeparam name="T">The type of the set variant callback.</typeparam>
        /// <param name="variant">The optional variant index.</param>
        /// <returns>A set variant callback of type <typeparamref name="T"/>.</returns>
        public static SetVariantCallback<T> BuildSetVariantCallback<T>(int variant = -1)
        {
            return new SetVariantCallback<T>(variant);
        }
        
        #endregion
    }
}