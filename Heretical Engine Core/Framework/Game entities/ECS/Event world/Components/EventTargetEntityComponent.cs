using System;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a component for specifying the target entity of an event.
    /// </summary>
    [NetworkEventComponent]
    public struct EventTargetEntityComponent
    {
        /// <summary>
        /// The global unique identifier (GUID) of the target entity.
        /// </summary>
        public Guid TargetGUID;
    }
}