using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a component that holds information about a server entity.
    /// </summary>
    [Component("Registry world")]
    [IdentityComponent]
    public struct ServerEntityComponent
    {
        /// <summary>
        /// Gets or sets the entity of the server.
        /// </summary>
        public Entity ServerEntity;

        /// <summary>
        /// Gets or sets the prototype ID of the server entity.
        /// </summary>
        public string PrototypeID;
    }
}