using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Wrappers
{
    public class DelegateWrapperSingleArgGeneric<TValue>
        : IInvokableSingleArgGeneric<TValue>,
          IInvokableSingleArg
    {
        private readonly Action<TValue> @delegate;
        
        private readonly IFormatLogger logger;

        public DelegateWrapperSingleArgGeneric(
            Action<TValue> @delegate,
            IFormatLogger logger = null)
        {
            this.@delegate = @delegate;

            this.logger = logger;
        }

        #region IInvokableSingleArgGeneric
        
        public void Invoke(TValue argument)
        {
            @delegate?.Invoke(argument);
        }

        public void Invoke(object argument)
        {
            @delegate?.Invoke((TValue)argument);
        }
        
        #endregion

        #region IInvokableSingleArg
        
        public void Invoke<TArgument>(TArgument value)
        {
            //LOL, pattern matching to the rescue of converting TArgument to TValue
            switch (value)
            {
                case TValue tValue:

                    @delegate.Invoke(tValue);

                    break;

                default:

                    logger?.ThrowException<DelegateWrapperSingleArgGeneric<TValue>>(
                        $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\"");

                    break;
            }
        }

        public void Invoke(Type valueType, object value)
        {
            //LOL, pattern matching to the rescue of converting TArgument to TValue
            switch (value)
            {
                case TValue tValue:

                    @delegate.Invoke(tValue);

                    break;

                default:

                    logger?.ThrowException<DelegateWrapperSingleArgGeneric<TValue>>(
                        $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\"");

                    break;
            }
        }
        
        #endregion
    }
}