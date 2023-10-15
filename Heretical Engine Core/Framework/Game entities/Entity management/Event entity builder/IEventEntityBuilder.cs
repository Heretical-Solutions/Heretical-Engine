using System;
using System.Numerics;

using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents an entity builder for creating event entities.
    /// </summary>
    public interface IEventEntityBuilder
    {
        /// <summary>
        /// Creates a new event entity.
        /// </summary>
        /// <param name="eventEntity">The created event entity.</param>
        /// <returns>The event entity builder.</returns>
        IEventEntityBuilder NewEvent(out Entity eventEntity);

        /// <summary>
        /// Sets the position where the event happened.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <param name="position">The position where the event happened.</param>
        /// <returns>The event entity builder.</returns>
        IEventEntityBuilder HappenedAtPosition(
            Entity eventEntity,
            Vector3 position);

        /// <summary>
        /// Sets the entity that caused the event.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <param name="sourceEntity">The entity that caused the event.</param>
        /// <returns>The event entity builder.</returns>
        IEventEntityBuilder CausedByEntity(
            Entity eventEntity,
            Guid sourceEntity);

        /// <summary>
        /// Sets the entity that the event is targeted at.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <param name="targetEntity">The entity that the event is targeted at.</param>
        /// <returns>The event entity builder.</returns>
        IEventEntityBuilder TargetedAtEntity(
            Entity eventEntity,
            Guid targetEntity);

        /// <summary>
        /// Sets the position that the event is targeted at.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <param name="position">The position that the event is targeted at.</param>
        /// <returns>The event entity builder.</returns>
        IEventEntityBuilder TargetedAtPosition(
            Entity eventEntity,
            Vector3 position);

        /// <summary>
        /// Sets the time when the event happened.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <param name="ticks">The time when the event happened in ticks.</param>
        /// <returns>The event entity builder.</returns>
        IEventEntityBuilder HappenedAtTime(
            Entity eventEntity,
            long ticks);

        /// <summary>
        /// Sets the data associated with the event.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <param name="eventEntity">The event entity.</param>
        /// <param name="data">The data associated with the event.</param>
        /// <returns>The event entity builder.</returns>
        IEventEntityBuilder WithData<TData>(
            Entity eventEntity,
            TData data);

        /// <summary>
        /// Sets that the host should be notified of the event.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <returns>The event entity builder.</returns>
        IEventEntityBuilder HostShouldBeNotified(Entity eventEntity);

        /// <summary>
        /// Sets that the players should be notified of the event.
        /// </summary>
        /// <param name="eventEntity">The event entity.</param>
        /// <returns>The event entity builder.</returns>
        IEventEntityBuilder PlayersShouldBeNotified(Entity eventEntity);
    }
}