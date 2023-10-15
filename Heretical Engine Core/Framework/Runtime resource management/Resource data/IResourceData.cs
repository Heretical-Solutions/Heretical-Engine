using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents resource data.
    /// </summary>
    public interface IResourceData
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

        /// <summary>
        /// Adds a variant to the resource.
        /// </summary>
        /// <param name="variant">The variant data to add.</param>
        /// <param name="progress">An optional progress reporter for tracking the add operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddVariant(
            IResourceVariantData variant,
            IProgress<float> progress = null);

        /// <summary>
        /// Removes a variant from the resource.
        /// </summary>
        /// <param name="variantHash">The hash of the variant to remove.</param>
        /// <param name="progress">An optional progress reporter for tracking the remove operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveVariant(
            int variantHash,
            IProgress<float> progress = null);
    }
}