using HereticalSolutions.Repositories;

namespace HereticalSolutions.Synchronization
{
    public interface ISynchronizable
    {
        SynchronizationDescriptor Descriptor { get; }

        IReadOnlyObjectRepository Metadata { get; }
    }
}