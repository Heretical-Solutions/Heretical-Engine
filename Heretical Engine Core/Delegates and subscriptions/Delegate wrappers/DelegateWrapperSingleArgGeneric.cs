using System;

namespace HereticalSolutions.Delegates.Wrappers
{
    /// <summary>
    /// Wrapper class for a delegate with a single generic argument.
    /// </summary>
    /// <typeparam name="TValue">The type of the argument.</typeparam>
    public class DelegateWrapperSingleArgGeneric<TValue>
        : IInvokableSingleArgGeneric<TValue>,
          IInvokableSingleArg
    {
        private readonly Action<TValue> @delegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateWrapperSingleArgGeneric{TValue}"/> class.
        /// </summary>
        /// <param name="delegate">The delegate to wrap.</param>
        public DelegateWrapperSingleArgGeneric(Action<TValue> @delegate)
        {
            this.@delegate = @delegate;
        }

        #region IInvokableSingleArgGeneric
        
        /// <summary>
        /// Invokes the wrapped delegate passing the specified argument of type TValue.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        public void Invoke(TValue argument)
        {
            @delegate?.Invoke(argument);
        }

        /// <summary>
        /// Invokes the wrapped delegate passing the specified argument of type object.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        public void Invoke(object argument)
        {
            @delegate?.Invoke((TValue)argument);
        }
        
        #endregion

        #region IInvokableSingleArg
        
        /// <summary>
        /// Invokes the wrapped delegate passing the specified argument of type TArgument.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="value">The argument value.</param>
        /// <exception cref="Exception">Thrown when the argument type is not compatible with TValue.</exception>
        public void Invoke<TArgument>(TArgument value)
        {
            if (!(typeof(TArgument).Equals(typeof(TValue))))
                throw new Exception($"[DelegateWrapperSingleArgGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{typeof(TArgument).ToString()}\"");
			
            Invoke((object)value); //It doesn't want to convert TArgument into TValue. Bastard
        }

        /// <summary>
        /// Invokes the wrapped delegate passing the specified argument of the specified type.
        /// </summary>
        /// <param name="valueType">The type of the argument.</param>
        /// <param name="value">The argument value.</param>
        /// <exception cref="Exception">Thrown when the argument type is not compatible with TValue.</exception>
        public void Invoke(Type valueType, object value)
        {
            if (!(valueType.Equals(typeof(TValue))))
                throw new Exception($"[DelegateWrapperSingleArgGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{valueType.ToString()}\"");
			
            Invoke(value); //It doesn't want to convert TArgument into TValue. Bastard
        }
        
        #endregion
    }
}