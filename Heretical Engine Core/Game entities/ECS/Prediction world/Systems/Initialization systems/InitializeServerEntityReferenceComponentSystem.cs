using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.GameEntities
{
    public class InitializeServerEntityReferenceComponentSystem : ISystem<Entity>
    {
        private readonly IEntityManager entityManager;
        
        public InitializeServerEntityReferenceComponentSystem(
            IEntityManager entityManager)
        {
            this.entityManager = entityManager;
        }

        public bool IsEnabled { get; set; } = true;

        void ISystem<Entity>.Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<ServerEntityReferenceComponent>())
                return;

            ref ServerEntityReferenceComponent serverEntityReferenceComponent = ref entity.Get<ServerEntityReferenceComponent>();

            var guid = entity.Get<GameEntityComponent>().GUID;

            var registryEntity = entityManager.GetEntity(guid);

            if (!registryEntity.Has<ServerEntityComponent>())
                return;
            
            var serverEntity = registryEntity.Get<ServerEntityComponent>().ServerEntity;

            serverEntityReferenceComponent.ServerEntity = serverEntity;
        }

        /// <summary>
        /// Disposes the system.
        /// </summary>
        public void Dispose()
        {
        }
    }
}