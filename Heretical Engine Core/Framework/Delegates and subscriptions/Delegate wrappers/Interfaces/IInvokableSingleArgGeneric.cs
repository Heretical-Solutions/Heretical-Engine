namespace HereticalSolutions.Delegates
{
    public interface IInvokableSingleArgGeneric<TValue>
    {
        void Invoke(TValue arg);

        void Invoke(object arg);
    }
}