namespace HereticalSolutions.MVVM
{
    #region Command delegate

    /// <summary>
    /// Represents a delegate that is used for commands without any parameters.
    /// </summary>
    public delegate void CommandDelegate();

    /// <summary>
    /// Represents a delegate that is used for commands with parameters.
    /// </summary>
    /// <param name="args">The arguments to be passed to the command.</param>
    public delegate void CommandWithArgsDelegate(params object[] args);

    #endregion
}