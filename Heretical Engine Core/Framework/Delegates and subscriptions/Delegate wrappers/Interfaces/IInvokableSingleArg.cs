namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for invoking a method with a single argument
    /// </summary>
    public interface IInvokableSingleArg
    {
        /// <summary>
        /// Invokes the method with the specified argument
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument</typeparam>
        /// <param name="value">The argument value</param>
        void Invoke<TArgument>(TArgument value);

        /// <summary>
        /// Invokes the method with the specified argument
        /// </summary>
        /// <param name="valueType">The type of the argument</param>
        /// <param name="value">The argument value.</param>
        void Invoke(Type valueType, object value);
    }
}