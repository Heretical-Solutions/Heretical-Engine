namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents an interface for objects that contain an event entity builder.
    /// </summary>
    public interface IContainsEventEntityBuilder
    {
        /// <summary>
        /// Gets the event entity builder.
        /// </summary>
        IEventEntityBuilder EventEntityBuilder { get; }
    }
}