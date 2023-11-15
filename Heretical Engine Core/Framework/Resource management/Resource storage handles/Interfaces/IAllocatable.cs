using System;
using System.Threading.Tasks;

namespace HereticalSolutions.ResourceManagement
{
    public interface IAllocatable
    {
        bool Allocated { get; }

        Task Allocate(IProgress<float> progress = null);

        Task Free(IProgress<float> progress = null);
    }
}