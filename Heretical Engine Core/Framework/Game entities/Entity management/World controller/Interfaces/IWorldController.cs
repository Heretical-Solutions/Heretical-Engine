using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.GameEntities
{
    public interface IWorldController
    {
        World World { get; }

        void Initialize(
            ISystem<Entity> resolveSystems,
            ISystem<Entity> initializationSystems,
            ISystem<Entity> deinitializationSystems);
        
        bool AddEntity(
            Entity registryEntity,
            string prototypeID);

        void DespawnEntity(
            Entity registryEntity);

        void ReplaceEntity(
            Entity registryEntity,
            string prototypeID);
    }
}