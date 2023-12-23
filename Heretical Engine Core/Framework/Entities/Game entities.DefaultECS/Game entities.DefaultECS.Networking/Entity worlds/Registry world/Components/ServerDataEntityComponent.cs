using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    [Component("Registry world")]
    [IdentityComponent]
    public struct ServerDataEntityComponent
    {
        public Entity ServerDataEntity;

        public string PrototypeID;
    }
}