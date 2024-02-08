namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for invoking a method with a single argument of type <typeparamref name="TValue"/>
    /// </summary>
    /// <typeparam name="TValue">The type of the argument</typeparam>
    public interface IInvokableSingleArgGeneric<TValue>
    {
        /// <summary>
        /// Invokes the method with the specified argument of type <typeparamref name="TValue"/>
        /// </summary>
        /// <param name="arg">The argument to pass to the method</param>
        void Invoke(TValue arg);

        /// <summary>
        /// Invokes the method with the specified argument of type <see cref="object"/>
        /// </summary>
        /// <param name="arg">The argument to pass to the method</param>
        void Invoke(object arg);
    }
}