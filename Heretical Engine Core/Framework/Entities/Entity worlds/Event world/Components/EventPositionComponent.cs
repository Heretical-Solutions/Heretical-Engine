using System.Numerics;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a network event position component.
    /// </summary>
    [NetworkEventComponent]
    public struct EventPositionComponent
    {
        /// <summary>
        /// Gets or sets the position of the component.
        /// </summary>
        public Vector3 Position;
    }
}