namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for invoking a method with multiple arguments
    /// </summary>
    public interface IInvokableMultipleArgs
    {
        /// <summary>
        /// Invokes the method with the specified arguments.
        /// </summary>
        /// <param name="args">The arguments to pass to the method</param>
        void Invoke(object[] args);
    }
}