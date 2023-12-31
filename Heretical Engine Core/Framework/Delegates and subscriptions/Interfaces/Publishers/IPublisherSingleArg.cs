namespace HereticalSolutions.Delegates
{
    public interface IPublisherSingleArg
    {
        void Publish<TValue>(TValue value);

        void Publish(
            Type valueType,
            object value);
    }
}