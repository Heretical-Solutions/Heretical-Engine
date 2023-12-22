using System.Numerics;

using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a class for building event entities.
    /// </summary>
    public class EventEntityBuilder
        : IEventEntityBuilder<Entity>
    {
        protected readonly World eventWorld;

        public EventEntityBuilder(
            World eventWorld)
        {
            this.eventWorld = eventWorld;
        }

        public IEventEntityBuilder<Entity> NewEvent(out Entity eventEntity)
        {
            eventEntity = eventWorld.CreateEntity();

            return this;
        }

        public IEventEntityBuilder<Entity> HappenedAtPosition(
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

        public IEventEntityBuilder<Entity> CausedByEntity(
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

        public IEventEntityBuilder<Entity> TargetedAtEntity(
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

        public IEventEntityBuilder<Entity> TargetedAtPosition(
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

        public IEventEntityBuilder<Entity> HappenedAtTime(
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

        public IEventEntityBuilder<Entity> WithData<TData>(
            Entity eventEntity,
            TData data)
        {
            eventEntity
                .Set<TData>(data);
            
            return this;
        }
    }
}