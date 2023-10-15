namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Defines an interface for invoking a method with multiple arguments.
    /// </summary>
    public interface IInvokableMultipleArgs
    {
        /// <summary>
        /// Invokes the method with the specified arguments.
        /// </summary>
        /// <param name="args">An array of arguments to be passed to the method.</param>
        void Invoke(object[] args);
    }
}