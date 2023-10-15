using System;

namespace HereticalSolutions.Delegates.Wrappers
{
    /// <summary>
    /// Represents a wrapper class for a delegate with no arguments.
    /// </summary>
    public class DelegateWrapperNoArgs : IInvokableNoArgs
    {
        private readonly Action @delegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateWrapperNoArgs"/> class.
        /// </summary>
        /// <param name="delegate">The delegate to wrap.</param>
        public DelegateWrapperNoArgs(Action @delegate)
        {
            this.@delegate = @delegate;
        }

        /// <summary>
        /// Invokes the wrapped delegate, if it is not null.
        /// </summary>
        public void Invoke()
        {
            @delegate?.Invoke();
        }
    }
}