using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Wrappers
{
    public class DelegateWrapperSingleArgGeneric<TValue>
        : IInvokableSingleArgGeneric<TValue>,
          IInvokableSingleArg
    {
        private readonly Action<TValue> @delegate;
        
        private readonly ILogger logger;

        public DelegateWrapperSingleArgGeneric(
            Action<TValue> @delegate,
            ILogger logger = null)
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

                    throw new Exception(
                        logger.TryFormat<DelegateWrapperSingleArgGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
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

                    throw new Exception(
                        logger.TryFormat<DelegateWrapperSingleArgGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
            }
        }
        
        #endregion
    }
}