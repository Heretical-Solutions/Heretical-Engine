namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a component that stores the identity of an entity in the registry.
    /// </summary>
    [Component("Registry world")]
    [IdentityComponent]
    public struct RegistryEntityComponent //Dear me. Remind me why am I using this again?
    {
        /// <summary>
        /// Gets or sets the prototype ID of the entity.
        /// </summary>
        public string PrototypeID;
    }
}