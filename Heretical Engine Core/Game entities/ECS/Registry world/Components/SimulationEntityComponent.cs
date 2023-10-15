using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Component representing a simulation entity in the game.
    /// </summary>
    [Component("Registry world")]
    public struct SimulationEntityComponent 
    {
        /// <summary>
        /// The simulation entity.
        /// </summary>
        public Entity SimulationEntity;

        /// <summary>
        /// The prototype ID.
        /// </summary>
        public string PrototypeID;
    }
}