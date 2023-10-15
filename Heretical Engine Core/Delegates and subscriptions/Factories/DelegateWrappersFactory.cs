using System;

using HereticalSolutions.Delegates.Wrappers;

namespace HereticalSolutions.Delegates.Factories
{
    /// <summary>
    /// A factory class for creating delegate wrappers.
    /// </summary>
    public static partial class DelegatesFactory
    {
        #region Delegate wrappers

        /// <summary>
        /// Builds a delegate wrapper for delegates with no arguments.
        /// </summary>
        /// <param name="delegate">The delegate to be wrapped.</param>
        /// <returns>A new instance of the DelegateWrapperNoArgs class.</returns>
        public static DelegateWrapperNoArgs BuildDelegateWrapperNoArgs(Action @delegate)
        {
            return new DelegateWrapperNoArgs(@delegate);
        }
        
        /// <summary>
        /// Builds a delegate wrapper for delegates with a single argument.
        /// </summary>
        /// <typeparam name="TValue">The type of the delegate argument.</typeparam>
        /// <param name="delegate">The delegate to be wrapped.</param>
        /// <returns>A new instance of the DelegateWrapperSingleArgGeneric&lt;TValue&gt; class.</returns>
        public static IInvokableSingleArg BuildDelegateWrapperSingleArg<TValue>(Action<TValue> @delegate)
        {
            return new DelegateWrapperSingleArgGeneric<TValue>(@delegate);
        }
        
        /// <summary>
        /// Builds a delegate wrapper for delegates with a single argument.
        /// </summary>
        /// <typeparam name="TValue">The type of the delegate argument.</typeparam>
        /// <param name="delegate">The delegate to be wrapped.</param>
        /// <returns>A new instance of the DelegateWrapperSingleArgGeneric&lt;TValue&gt; class.</returns>
        public static DelegateWrapperSingleArgGeneric<TValue> BuildDelegateWrapperSingleArgGeneric<TValue>(Action<TValue> @delegate)
        {
            return new DelegateWrapperSingleArgGeneric<TValue>(@delegate);
        }
        
        /// <summary>
        /// Builds a delegate wrapper for delegates with multiple arguments.
        /// </summary>
        /// <param name="delegate">The delegate to be wrapped.</param>
        /// <returns>A new instance of the DelegateWrapperMultipleArgs class.</returns>
        public static DelegateWrapperMultipleArgs BuildDelegateWrapperMultipleArgs(Action<object[]> @delegate)
        {
            return new DelegateWrapperMultipleArgs(@delegate);
        }

        #endregion
    }
}