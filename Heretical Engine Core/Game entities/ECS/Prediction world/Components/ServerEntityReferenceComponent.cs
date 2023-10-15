using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    [Component("Prediction world/References")]
    public struct ServerEntityReferenceComponent
    {
        public Entity ServerEntity;
    }
}