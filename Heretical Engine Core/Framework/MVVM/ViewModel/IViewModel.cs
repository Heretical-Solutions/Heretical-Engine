using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.MVVM
{
    /// <summary>
    /// Represents a view model in the MVVM pattern.
    /// </summary>
    public interface IViewModel : ILifetimeable
    {
        #region Get observable

        /// <summary>
        /// Retrieves an observable property with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the observable property value.</typeparam>
        /// <param name="key">The key used to retrieve the observable property.</param>
        /// <param name="observable">When this method returns, contains the observable property associated with the specified key, if found; otherwise, null.</param>
        /// <returns><c>true</c> if the observable property was retrieved successfully; otherwise, <c>false</c>.</returns>
        bool GetObservable<T>(string key, out IObservableProperty<T> observable);

        #endregion

        #region Get command

        /// <summary>
        /// Retrieves a command delegate with the specified key.
        /// </summary>
        /// <param name="key">The key used to retrieve the command delegate.</param>
        /// <returns>The command delegate associated with the specified key.</returns>
        CommandDelegate GetCommand(string key);
        
        /// <summary>
        /// Retrieves a command delegate with the specified key that accepts arguments.
        /// </summary>
        /// <param name="key">The key used to retrieve the command delegate.</param>
        /// <returns>The command delegate associated with the specified key.</returns>
        CommandWithArgsDelegate GetCommandWithArguments(string key);
        
        #endregion
    }
}