using System.Collections.Generic;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents read-only resource data.
    /// </summary>
    public interface IReadOnlyResourceData
    {
        /// <summary>
        /// Gets the descriptor of the resource.
        /// </summary>
        ResourceDescriptor Descriptor { get; }
        
        /// <summary>
        /// Gets the default variant data of the resource.
        /// </summary>
        IResourceVariantData DefaultVariant { get; }
        
        /// <summary>
        /// Gets the variant data of the resource based on the variant ID hash.
        /// </summary>
        /// <param name="variantIDHash">The hash of the variant ID.</param>
        /// <returns>The variant data associated with the specified variant ID hash.</returns>
        IResourceVariantData GetVariant(int variantIDHash);

        /// <summary>
        /// Gets the variant data of the resource based on the variant ID.
        /// </summary>
        /// <param name="variantID">The ID of the variant.</param>
        /// <returns>The variant data associated with the specified variant ID.</returns>
        IResourceVariantData GetVariant(string variantID);
        
        /// <summary>
        /// Gets the variant hashes available for the resource.
        /// </summary>
        IEnumerable<int> VariantHashes { get; }
    }
}