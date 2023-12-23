using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.GameEntities
{
    public class DisposeProcessedEventsSystem : AEntitySetSystem<float>
    {
        public DisposeProcessedEventsSystem(World eventWorld)
            : base(
                eventWorld
                    .GetEntities()
                    .With<EventProcessedComponent>()
                    .AsSet())
        {
        }

        protected override void Update(float deltaTime, in Entity entity)
        {
            entity.Set<DespawnComponent>();
        }
    }
}