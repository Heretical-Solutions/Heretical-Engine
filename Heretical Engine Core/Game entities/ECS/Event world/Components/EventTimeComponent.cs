namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a time component for network events.
    /// </summary>
    [NetworkEventComponent]
    public struct EventTimeComponent
    {
        /// <summary>
        /// The number of ticks for the time component.
        /// </summary>
        public long Ticks;
    }
}