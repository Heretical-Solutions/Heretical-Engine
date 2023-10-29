using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents resource data.
    /// </summary>
    public interface IResourceData
        : IReadOnlyResourceData
    {
        Task AddVariant(
            IResourceVariantData variant,
            bool allocate = true,
            IProgress<float> progress = null);

        Task RemoveVariant(
            int variantHash,
            bool free = true,
            IProgress<float> progress = null);

        Task RemoveVariant(
            string variantID,
            bool free = true,
            IProgress<float> progress = null);

        Task ClearAllVariants(
            bool free = true,
            IProgress<float> progress = null);

        IReadOnlyResourceData ParentResource { set; }

        Task AddNestedResource(
            IReadOnlyResourceData nestedResource,
            IProgress<float> progress = null);

        Task RemoveNestedResource(
            int nestedResourceHash,
            bool free = true,
            IProgress<float> progress = null);

        Task RemoveNestedResource(
            string nestedResourceID,
            bool free = true,
            IProgress<float> progress = null);

        Task ClearAllNestedResources(
            bool free = true,
            IProgress<float> progress = null);

        Task Clear(
            bool free = true,
            IProgress<float> progress = null);
    }
}