namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Data Transfer Object for the event entities buffer used in the ECS system.
    /// </summary>
    public class ECSEventsBufferDTO
    {
        /// <summary>
        /// Array of event entities.
        /// </summary>
        public ECSEventEntityDTO[] EventEntities;
    }
}