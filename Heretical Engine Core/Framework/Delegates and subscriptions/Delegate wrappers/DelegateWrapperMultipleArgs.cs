using System;

namespace HereticalSolutions.Delegates.Wrappers
{
    /// <summary>
    /// Represents a wrapper class for an Action delegate that takes multiple arguments.
    /// Implements the <see cref="HereticalSolutions.Delegates.Wrappers.IInvokableMultipleArgs"/> interface.
    /// </summary>
    public class DelegateWrapperMultipleArgs : IInvokableMultipleArgs
    {
        private readonly Action<object[]> @delegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateWrapperMultipleArgs"/> class.
        /// </summary>
        /// <param name="delegate">The Action delegate to wrap.</param>
        public DelegateWrapperMultipleArgs(Action<object[]> @delegate)
        {
            this.@delegate = @delegate;
        }

        /// <summary>
        /// Invokes the wrapped delegate with the specified arguments.
        /// </summary>
        /// <param name="arguments">The arguments to pass to the delegate.</param>
        public void Invoke(object[] arguments)
        {
            @delegate?.Invoke(arguments);
        }
    }
}