using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a component that stores the view entity and prototype ID for an entity.
    /// </summary>
    [Component("Registry world")]
    [IdentityComponent]
    public struct ViewEntityComponent
    {
        /// <summary>
        /// The view entity associated with this component.
        /// </summary>
        public Entity ViewEntity;

        /// <summary>
        /// The prototype ID of the entity.
        /// </summary>
        public string PrototypeID;
    }
}