using System;
using System.Threading.Tasks;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.ResourceManagement
{
    public class PreallocatedResourceStorageHandle
        : AReadOnlyResourceStorageHandle<object>
    {
        private object value;

        public PreallocatedResourceStorageHandle(
            object value,
            ApplicationContext context)
            : base(
                context)
        {
            this.value = value;
        }

        protected override object AllocateResource(
            IProgress<float> progress = null)
        {
            return value;
        }

        protected override void FreeResource(
            object resource,
            IProgress<float> progress = null)
        {
        }
    }
}