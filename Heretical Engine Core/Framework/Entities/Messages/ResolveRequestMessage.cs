using HereticalSolutions.Messaging;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a request to resolve a game entity.
    /// </summary>
    public class ResolveRequestMessage : IMessage
    {
        public object Source { get; private set; }

        /// <summary>
        /// Gets the prototype ID of the resolve request.
        /// </summary>
        public string PrototypeID { get; private set; }

        public EEntityAuthoringPresets AuthoringPreset { get; private set; }

        /// <summary>
        /// Writes the resolve request with the specified arguments.
        /// </summary>
        /// <param name="args">The arguments to write.</param>
        public void Write(object[] args)
        {
            Source = args[0];

            PrototypeID = (string)args[1];

            AuthoringPreset = (EEntityAuthoringPresets)args[2];
        }
    }
}