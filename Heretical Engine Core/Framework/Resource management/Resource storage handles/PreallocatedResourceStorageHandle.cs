using System;
using System.Threading.Tasks;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.ResourceManagement
{
    public class PreallocatedResourceStorageHandle<TResource>
        : AReadOnlyResourceStorageHandle<TResource>
    {
        private TResource value;

        public PreallocatedResourceStorageHandle(
            TResource value,
            ApplicationContext context)
            : base(
                context)
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