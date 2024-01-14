using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
    public class PreallocatedResourceStorageHandle<TResource>
        : AReadOnlyResourceStorageHandle<TResource>
    {
        private TResource value;

        public PreallocatedResourceStorageHandle(
            TResource value,
            IRuntimeResourceManager runtimeResourceManager,
            IFormatLogger logger = null)
            : base(
                runtimeResourceManager,
                logger)
        {
            this.value = value;
        }
        protected override async Task<TResource> AllocateResource(
            IProgress<float> progress = null)
        {
            return value;
        }

        protected override async Task FreeResource(
            TResource resource,
            IProgress<float> progress = null)
        {
        }
    }
}