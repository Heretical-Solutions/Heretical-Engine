namespace HereticalSolutions.Delegates
{
    public interface IPublisherSingleArgGeneric<TValue>
    {
        void Publish(TValue value);
    }
}