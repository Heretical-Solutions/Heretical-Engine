using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents resource data that can be read and modified.
    /// </summary>
    public class ResourceData
        : IResourceData
    {
        private readonly IRepository<int, string> variantIDHashToID;

        private readonly IRepository<int, IResourceVariantData> variantsRepository;

        private IResourceVariantData defaultVariant;


        private readonly IRepository<int, string> nestedResourceIDHashToID;

        private readonly IRepository<int, IReadOnlyResourceData> nestedResourcesRepository;


        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceData"/> class.
        /// </summary>
        /// <param name="descriptor">The descriptor of the resource.</param>
        /// <param name="variantsRepository">The repository containing the resource variants.</param>
        public ResourceData(
            ResourceDescriptor descriptor,
            IRepository<int, string> variantIDHashToID,
            IRepository<int, IResourceVariantData> variantsRepository,
            IRepository<int, string> nestedResourceIDHashToID,
            IRepository<int, IReadOnlyResourceData> nestedResourcesRepository)
        {
            Descriptor = descriptor;
            
            this.variantIDHashToID = variantIDHashToID;

            this.variantsRepository = variantsRepository;

            this.nestedResourceIDHashToID = nestedResourceIDHashToID;

            this.nestedResourcesRepository = nestedResourcesRepository;


            defaultVariant = null;
        }

        #region IResourceData

        #region IReadOnlyResourceData

        /// <summary>
        /// Gets the descriptor of the resource.
        /// </summary>
        public ResourceDescriptor Descriptor { get; private set; }

        #region IContainsResourceVariants

        /// <summary>
        /// Gets the default variant data of the resource.
        /// </summary>
        public IResourceVariantData DefaultVariant => defaultVariant;

        public bool HasVariant(int variantIDHash)
        {
            return variantsRepository.Has(variantIDHash);
        }

        public bool HasVariant(string variantID)
        {
            return HasVariant(variantID.AddressToHash());
        }

        /// <summary>
        /// Gets the variant data of the resource based on the variant ID hash.
        /// </summary>
        /// <param name="variantIDHash">The hash of the variant ID.</param>
        /// <returns>The variant data associated with the specified variant ID hash.</returns>
        public IResourceVariantData GetVariant(int variantIDHash)
        {
            if (!variantsRepository.TryGet(variantIDHash, out var variant))
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
        public IEnumerable<int> VariantIDHashes => variantsRepository.Keys;

        public IEnumerable<string> VariantIDs => variantIDHashToID.Values;

        public IEnumerable<IResourceVariantData> AllVariants => variantsRepository.Values;

        #endregion

        #region IContainsNestedResources

        public bool HasNestedResource(int nestedResourceIDHash)
        {
            return nestedResourcesRepository.Has(nestedResourceIDHash);
        }

        public bool HasNestedResource(string nestedResourceID)
        {
            return HasNestedResource(nestedResourceID.AddressToHash());
        }

        public IReadOnlyResourceData GetNestedResource(int nestedResourceIDHash)
        {
            if (!nestedResourcesRepository.TryGet(nestedResourceIDHash, out var nestedResource))
                return null;

            return nestedResource;
        }

        public IReadOnlyResourceData GetNestedResource(string nestedResourceID)
        {
            return GetNestedResource(nestedResourceID.AddressToHash());
        }

        public IEnumerable<int> NestedResourceIDHashes => nestedResourcesRepository.Keys;

        public IEnumerable<string> NestedResourceIDs => nestedResourceIDHashToID.Values;

        public IEnumerable<IReadOnlyResourceData> AllNestedResources => nestedResourcesRepository.Values;

        #endregion


        #endregion

        /// <summary>
        /// Adds a variant to the resource.
        /// </summary>
        /// <param name="variant">The variant data to add.</param>
        /// <param name="progress">An optional progress reporter for tracking the add operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddVariant(
            IResourceVariantData variant,
            bool allocate = true,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            if (!variantsRepository.TryAdd(
                variant.Descriptor.VariantIDHash,
                variant))
            {
                progress?.Report(1f);

                return;
            }

            variantIDHashToID.TryAdd(
                variant.Descriptor.VariantIDHash,
                variant.Descriptor.VariantID);

            UpdateDefaultVariant();

            if (allocate)
                await variant.StorageHandle.Allocate(progress);

            progress?.Report(1f);
        }

        public async Task RemoveVariant(
            int variantHash = -1,
            bool free = true,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            if (!variantsRepository.TryGet(variantHash, out var variant))
            {
                progress?.Report(1f);

                return;
            }

            variantIDHashToID.TryRemove(variantHash);

            variantsRepository.TryRemove(variantHash);

            UpdateDefaultVariant();

            if (free)
                await variant.StorageHandle.Free(progress);

            progress?.Report(1f);
        }

        public async Task RemoveVariant(
            string variantID,
            bool free = true,
            IProgress<float> progress = null)
        {
            await RemoveVariant(
                variantID.AddressToHash(),
                free,
                progress);
        }

        private void UpdateDefaultVariant()
        {
            defaultVariant = null;

            int topPriority = int.MinValue;

            foreach (var hashID in variantsRepository.Keys)
            {
                var currentVariant = variantsRepository.Get(hashID);

                var currentPriority = currentVariant.Descriptor.Priority;

                if (currentPriority > topPriority)
                {
                    topPriority = currentPriority;

                    defaultVariant = currentVariant;
                }
            }
        }

        public async Task ClearAllVariants(
            bool free = true,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            int totalVariantsCount = variantsRepository.Count;

            int counter = 0;

            foreach (var key in variantsRepository.Keys)
            {
                if (free)
                {
                    IProgress<float> localProgress = null;

                    if (progress != null)
                    {
                        var localProgressInstance = new Progress<float>();

                        localProgressInstance.ProgressChanged += (sender, value) =>
                        {
                            progress.Report((float)counter / (float)totalVariantsCount + value / (float)totalVariantsCount);
                        };

                        localProgress = localProgressInstance;
                    }

                    await RemoveVariant(
                        key,
                        free,
                        localProgress);

                    counter++;

                    progress?.Report((float)counter / (float)totalVariantsCount);
                }
            }

            variantIDHashToID.Clear();

            variantsRepository.Clear();

            defaultVariant = null;

            progress?.Report(1f);
        }

        public async Task AddNestedResource(
            IReadOnlyResourceData nestedResource,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            if (!nestedResourcesRepository.TryAdd(
                nestedResource.Descriptor.IDHash,
                nestedResource))
            {
                progress?.Report(1f);

                return;
            }

            nestedResourceIDHashToID.TryAdd(
                nestedResource.Descriptor.IDHash,
                nestedResource.Descriptor.ID);

            progress?.Report(1f);
        }

        public async Task RemoveNestedResource(
            int nestedResourceHash = -1,
            bool free = true,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            if (!nestedResourcesRepository.TryGet(nestedResourceHash, out var nestedResource))
            {
                progress?.Report(1f);

                return;
            }

            nestedResourceIDHashToID.TryRemove(nestedResourceHash);

            nestedResourcesRepository.TryRemove(nestedResourceHash);

            if (free)
                await ((IResourceData)nestedResource).Clear(
                    free,
                    progress);

            progress?.Report(1f);
        }

        public async Task RemoveNestedResource(
            string nestedResourceID,
            bool free = true,
            IProgress<float> progress = null)
        {
            await RemoveNestedResource(
                nestedResourceID.AddressToHash(),
                free,
                progress);
        }

        public async Task ClearAllNestedResources(
            bool free = true,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            int totalNestedResourcesCount = nestedResourcesRepository.Count;

            int counter = 0;

            foreach (var key in nestedResourcesRepository.Keys)
            {
                if (free)
                {
                    IProgress<float> localProgress = null;

                    if (progress != null)
                    {
                        var localProgressInstance = new Progress<float>();

                        localProgressInstance.ProgressChanged += (sender, value) =>
                        {
                            progress.Report((float)counter / (float)totalNestedResourcesCount + value / (float)totalNestedResourcesCount);
                        };

                        localProgress = localProgressInstance;
                    }

                    await RemoveNestedResource(
                        key,
                        free,
                        localProgress);

                    counter++;

                    progress?.Report((float)counter / (float)totalNestedResourcesCount);
                }
            }

            nestedResourceIDHashToID.Clear();

            nestedResourcesRepository.Clear();

            progress?.Report(1f);
        }

        public async Task Clear(
            bool free = true,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            IProgress<float> localProgress = null;

            if (progress != null)
            {
                var localProgressInstance = new Progress<float>();

                localProgressInstance.ProgressChanged += (sender, value) =>
                {
                    progress.Report(value * 0.5f);
                };

                localProgress = localProgressInstance;
            }

            await ClearAllVariants(
                free,
                localProgress);

            progress?.Report(0.5f);

            localProgress = null;

            if (progress != null)
            {
                var localProgressInstance = new Progress<float>();

                localProgressInstance.ProgressChanged += (sender, value) =>
                {
                    progress.Report(value * 0.5f + 0.5f);
                };

                localProgress = localProgressInstance;
            }

            await ClearAllNestedResources(
                free,
                localProgress);

            progress?.Report(1f);
        }

        #endregion
    }
}