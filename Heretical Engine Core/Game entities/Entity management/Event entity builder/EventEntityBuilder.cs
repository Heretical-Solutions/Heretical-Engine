using System;
using System.Numerics;

using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a class for building event entities.
    /// </summary>
    public class EventEntityBuilder
        : IEventEntityBuilder
    {
        private readonly World eventWorld;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventEntityBuilder"/> class with the specified event world.
        /// </summary>
        /// <param name="eventWorld">The event world.</param>
        public EventEntityBuilder(
            World eventWorld)
        {
            this.eventWorld = eventWorld;
        }

        /// <summary>
        /// Creates a new event entity and sets an output parameter to the created entity.
        /// </summary>
        /// <param name="eventEntity">An output parameter to hold the created event entity.</param>
        /// <returns>The current instance of the <see cref="EventEntityBuilder"/>.</returns>
        public IEventEntityBuilder NewEvent(out Entity eventEntity)
        {
            eventEntity = eventWorld.CreateEntity();

            return this;
        }

        /// <summary>
        /// Sets the position of the event entity.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <param name="position">The position of the event entity.</param>
        /// <returns>The current instance of the <see cref="EventEntityBuilder"/>.</returns>
        public IEventEntityBuilder HappenedAtPosition(
            Entity eventEntity,
            Vector3 position)
        {
            eventEntity
                .Set<EventPositionComponent>(
                    new EventPositionComponent
                    {
                        Position = position
                    });

            return this;
        }

        /// <summary>
        /// Sets the source entity that caused the event.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <param name="sourceEntity">The GUID of the source entity.</param>
        /// <returns>The current instance of the <see cref="EventEntityBuilder"/>.</returns>
        public IEventEntityBuilder CausedByEntity(
            Entity eventEntity,
            Guid sourceEntity)
        {
            eventEntity
                .Set<EventSourceEntityComponent>(
                    new EventSourceEntityComponent
                    {
                        SourceGUID = sourceEntity
                    });

            return this;
        }

        /// <summary>
        /// Sets the target entity of the event.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <param name="targetEntity">The GUID of the target entity.</param>
        /// <returns>The current instance of the <see cref="EventEntityBuilder"/>.</returns>
        public IEventEntityBuilder TargetedAtEntity(
            Entity eventEntity,
            Guid targetEntity)
        {
            eventEntity
                .Set<EventTargetEntityComponent>(
                    new EventTargetEntityComponent
                    {
                        TargetGUID = targetEntity
                    });

            return this;
        }

        /// <summary>
        /// Sets the target position of the event.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <param name="position">The target position of the event.</param>
        /// <returns>The current instance of the <see cref="EventEntityBuilder"/>.</returns>
        public IEventEntityBuilder TargetedAtPosition(
            Entity eventEntity,
            Vector3 position)
        {
            eventEntity
                .Set<EventTargetPositionComponent>(
                    new EventTargetPositionComponent
                    {
                        Position = position
                    });

            return this;
        }

        /// <summary>
        /// Sets the time at which the event happened.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <param name="ticks">The ticks representing the time at which the event happened.</param>
        /// <returns>The current instance of the <see cref="EventEntityBuilder"/>.</returns>
        public IEventEntityBuilder HappenedAtTime(
            Entity eventEntity,
            long ticks)
        {
            eventEntity
                .Set<EventTimeComponent>(
                    new EventTimeComponent
                    {
                        Ticks = ticks
                    });

            return this;
        }

        /// <summary>
        /// Sets the data associated with the event.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <param name="eventEntity">The event entity.</param>
        /// <param name="data">The data associated with the event.</param>
        /// <returns>The current instance of the <see cref="EventEntityBuilder"/>.</returns>
        public IEventEntityBuilder WithData<TData>(
            Entity eventEntity,
            TData data)
        {
            eventEntity
                .Set<TData>(data);
            
            return this;
        }

        /// <summary>
        /// Sets that the host should be notified of the event.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <returns>The current instance of the <see cref="EventEntityBuilder"/>.</returns>
        public IEventEntityBuilder HostShouldBeNotified(Entity eventEntity)
        {
            eventEntity.Set<NotifyHostComponent>(new NotifyHostComponent());
            
            return this;
        }

        /// <summary>
        /// Sets that the players should be notified of the event.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <returns>The current instance of the <see cref="EventEntityBuilder"/>.</returns>
        public IEventEntityBuilder PlayersShouldBeNotified(Entity eventEntity)
        {
            eventEntity.Set<NotifyPlayersComponent>(new NotifyPlayersComponent());
            
            return this;
        }
    }
}