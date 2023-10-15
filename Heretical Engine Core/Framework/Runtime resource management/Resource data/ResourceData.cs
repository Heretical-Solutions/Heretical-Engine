using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents resource data that can be read and modified.
    /// </summary>
    public class ResourceData : IReadOnlyResourceData, IResourceData
    {
        private readonly IRepository<int, IResourceVariantData> variants;
        private IResourceVariantData defaultVariant;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceData"/> class.
        /// </summary>
        /// <param name="descriptor">The descriptor of the resource.</param>
        /// <param name="variants">The repository containing the resource variants.</param>
        public ResourceData(
            ResourceDescriptor descriptor,
            IRepository<int, IResourceVariantData> variants)
        {
            Descriptor = descriptor;
            
            this.variants = variants;
            
            defaultVariant = null;
        }

        #region IResourceData

        #region IReadOnlyResourceData

        /// <summary>
        /// Gets the descriptor of the resource.
        /// </summary>
        public ResourceDescriptor Descriptor { get; private set; }

        /// <summary>
        /// Gets the default variant data of the resource.
        /// </summary>
        public IResourceVariantData DefaultVariant => defaultVariant;

        /// <summary>
        /// Gets the variant data of the resource based on the variant ID hash.
        /// </summary>
        /// <param name="variantIDHash">The hash of the variant ID.</param>
        /// <returns>The variant data associated with the specified variant ID hash.</returns>
        public IResourceVariantData GetVariant(int variantIDHash)
        {
            if (!variants.TryGet(variantIDHash, out var variant))
                return null;

            return variant;
        }

        /// <summary>
        /// Gets the variant data of the resource based on the variant ID.
        /// </summary>
        /// <param name="variantID">The ID of the variant.</param>
        /// <returns>The variant data associated with the specified variant ID.</returns>
        public IResourceVariantData GetVariant(string variantID)
        {
            return GetVariant(variantID.AddressToHash());
        }

        /// <summary>
        /// Gets the variant hashes available for the resource.
        /// </summary>
        public IEnumerable<int> VariantHashes => variants.Keys;

        #endregion

        /// <summary>
        /// Adds a variant to the resource.
        /// </summary>
        /// <param name="variant">The variant data to add.</param>
        /// <param name="progress">An optional progress reporter for tracking the add operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddVariant(
            IResourceVariantData variant,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            if (!variants.TryAdd(variant.Descriptor.VariantIDHash, variant))
            {
                progress?.Report(1f);
                return;
            }

            UpdateDefaultVariant();

            await variant.StorageHandle.Allocate(progress);

            progress?.Report(1f);
        }

        /// <summary>
        /// Removes a variant from the resource.
        /// </summary>
        /// <param name="variantHash">The hash of the variant to remove.</param>
        /// <param name="progress">An optional progress reporter for tracking the remove operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveVariant(
            int variantHash = -1,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            if (!variants.TryGet(variantHash, out var variant))
            {
                progress?.Report(1f);
                return;
            }

            await variant.StorageHandle.Free();

            variants.TryRemove(variantHash);

            UpdateDefaultVariant();

            progress?.Report(1f);
        }

        private void UpdateDefaultVariant()
        {
            defaultVariant = null;
            
            int topPriority = -1;

            foreach (var hashID in variants.Keys)
            {
                var currentVariant = variants.Get(hashID);

                var currentPriority = currentVariant.Descriptor.Priority; 
                
                if (currentPriority > topPriority)
                {
                    topPriority = currentPriority;
                    
                    defaultVariant = currentVariant;
                }
            }
        }

        #endregion
    }
}