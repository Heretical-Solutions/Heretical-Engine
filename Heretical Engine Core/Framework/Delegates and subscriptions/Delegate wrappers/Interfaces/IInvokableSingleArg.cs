namespace HereticalSolutions.Delegates
{
    public interface IInvokableSingleArg
    {
        void Invoke<TArgument>(TArgument value);

        void Invoke(Type valueType, object value);
    }
}