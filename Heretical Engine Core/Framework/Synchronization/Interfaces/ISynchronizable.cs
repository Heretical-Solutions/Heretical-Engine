namespace HereticalSolutions.Synchronization
{
    public interface ISynchronizable
    {
        SynchronizationDescriptor Descriptor { get; }

        void Toggle(bool active);

        void Synchronize();
    }
}