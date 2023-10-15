using System;

namespace HereticalSolutions.Allocations.Factories
{
    /// <summary>
    /// A factory for generating ID allocations.
    /// </summary>
    public static class IDAllocationsFactory
    {
        /// <summary>
        /// Builds a new GUID.
        /// </summary>
        /// <returns>A new GUID.</returns>
        public static Guid BuildGUID()
        {
            // Generate a new GUID and return it
            return Guid.NewGuid();
        }

        /// <summary>
        /// Builds a random integer.
        /// </summary>
        /// <returns>A randomly generated integer.</returns>
        public static int BuildInt()
        {
            // Generate a random integer and return it
            return new Random().Next();
        }
    }
}