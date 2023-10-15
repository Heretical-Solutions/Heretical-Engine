using System;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a component that provides identity to a game entity.
    /// </summary>
    [Component("Identity")]
    public struct GameEntityComponent
    {
        /// <summary>
        /// The unique identifier for the game entity.
        /// </summary>
        public Guid GUID;
    }
}