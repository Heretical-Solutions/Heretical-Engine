using HereticalSolutions.Delegates.Wrappers;

using HereticalSolutions.Logging;

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
        
        public static IInvokableSingleArg BuildDelegateWrapperSingleArg<TValue>(
            Action<TValue> @delegate,
            IFormatLogger logger)
        {
            return new DelegateWrapperSingleArgGeneric<TValue>(
                @delegate,
                logger);
        }
        
        public static DelegateWrapperSingleArgGeneric<TValue> BuildDelegateWrapperSingleArgGeneric<TValue>(
            Action<TValue> @delegate,
            IFormatLogger logger)
        {
            return new DelegateWrapperSingleArgGeneric<TValue>(
                @delegate,
                logger);
        }
        
        public static DelegateWrapperMultipleArgs BuildDelegateWrapperMultipleArgs(Action<object[]> @delegate)
        {
            return new DelegateWrapperMultipleArgs(@delegate);
        }

        #endregion
    }
}