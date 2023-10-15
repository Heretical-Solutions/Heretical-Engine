using System;

using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    public interface IWorldControllerInternal
    {
        void InitializeEntityManagerInternal(
            IEntityManagerInternal entityManagerInternal);
        
        void SpawnEntityFromPrototype(
            Entity registryEntity);

        void SpawnAndResolveEntityFromPrototype(
            Entity registryEntity,
            object target);

        bool SpawnEntity(
            string prototypeID,
            Guid guid,
            out Entity entity);
    }
}