namespace HereticalSolutions.Messaging
{
    /// <summary>
    /// Represents an interface for a message.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Writes the message with the specified arguments.
        /// </summary>
        /// <param name="args">The arguments to be written.</param>
        void Write(object[] args);
    }
}