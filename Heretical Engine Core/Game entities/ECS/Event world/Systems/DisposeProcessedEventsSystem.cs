using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// System to dispose of processed events.
    /// </summary>
    public class DisposeProcessedEventsSystem : AEntitySetSystem<float>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisposeProcessedEventsSystem"/> class.
        /// </summary>
        /// <param name="eventWorld">The event world.</param>
        public DisposeProcessedEventsSystem(World eventWorld)
            : base(
                eventWorld
                    .GetEntities()
                    .With<EventProcessedComponent>()
                    .AsSet())
        {
        }

        /// <summary>
        /// Updates the system by disposing of processed events for each entity.
        /// </summary>
        /// <param name="deltaTime">The time passed between the last and current frame.</param>
        /// <param name="entity">The entity to process.</param>
        protected override void Update(float deltaTime, in Entity entity)
        {
            if (entity.Has<NotifyPlayersComponent>())
                return;

            if (entity.Has<NotifyHostComponent>())
                return;
            
            entity.Set<DespawnComponent>();
        }
    }
}