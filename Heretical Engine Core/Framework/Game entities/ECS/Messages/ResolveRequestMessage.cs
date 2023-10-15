using HereticalSolutions.Messaging;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a request to resolve a game entity.
    /// </summary>
    public class ResolveRequestMessage : IMessage
    {
        /// <summary>
        /// Gets the target object of the resolve request.
        /// </summary>
        public object Target { get; private set; }

        /// <summary>
        /// Gets the prototype ID of the resolve request.
        /// </summary>
        public string PrototypeID { get; private set; }

        /// <summary>
        /// Gets the authoring of the resolve request.
        /// </summary>
        public EEntityAuthoring Authoring { get; private set; }

        /// <summary>
        /// Writes the resolve request with the specified arguments.
        /// </summary>
        /// <param name="args">The arguments to write.</param>
        public void Write(object[] args)
        {
            Target = args[0];

            PrototypeID = (string)args[1];

            Authoring = (EEntityAuthoring)args[2];
        }
    }
}