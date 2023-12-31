namespace HereticalSolutions.Delegates.Wrappers
{
    public class DelegateWrapperMultipleArgs : IInvokableMultipleArgs
    {
        private readonly Action<object[]> @delegate;

        public DelegateWrapperMultipleArgs(Action<object[]> @delegate)
        {
            this.@delegate = @delegate;
        }

        public void Invoke(object[] arguments)
        {
            @delegate?.Invoke(arguments);
        }
    }
}