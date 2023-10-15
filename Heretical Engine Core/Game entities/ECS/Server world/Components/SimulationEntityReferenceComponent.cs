using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    [Component("Server world/References")]
    public struct SimulationEntityReferenceComponent
    {
        public Entity SimulationEntity;
    }
}