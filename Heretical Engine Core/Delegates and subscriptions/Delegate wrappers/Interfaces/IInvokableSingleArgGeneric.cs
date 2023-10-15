namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for invoking a delegate with a single argument of a generic type.
    /// </summary>
    /// <typeparam name="TValue">The type of the argument.</typeparam>
    public interface IInvokableSingleArgGeneric<TValue>
    {
        /// <summary>
        /// Invokes the delegate with the specified argument of type TValue.
        /// </summary>
        /// <param name="arg">The argument to pass to the delegate.</param>
        void Invoke(TValue arg);

        /// <summary>
        /// Invokes the delegate with the specified argument of type object.
        /// </summary>
        /// <param name="arg">The argument to pass to the delegate.</param>
        void Invoke(object arg);
    }
}