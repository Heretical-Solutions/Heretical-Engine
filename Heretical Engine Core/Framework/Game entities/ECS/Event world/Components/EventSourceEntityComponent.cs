using System;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a component that identifies the source entity for a network event.
    /// </summary>
    [NetworkEventComponent]
    public struct EventSourceEntityComponent
    {
        /// <summary>
        /// Gets or sets the GUID of the source entity.
        /// </summary>
        public Guid SourceGUID;
    }
}