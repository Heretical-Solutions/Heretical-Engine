using System;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents the descriptor of a resource variant.
    /// </summary>
    public struct ResourceVariantDescriptor
    {
        /// <summary>
        /// The ID of the resource variant.
        /// </summary>
        public string VariantID;

        /// <summary>
        /// The hash value of the resource variant ID.
        /// </summary>
        public int VariantIDHash;

        /// <summary>
        /// The priority of the resource variant.
        /// </summary>
        public int Priority;
        
        /// <summary>
        /// The source of the resource variant.
        /// </summary>
        public EResourceSources Source;

        /// <summary>
        /// The type of the resource associated with the variant.
        /// </summary>
        public Type ResourceType;
    }
}