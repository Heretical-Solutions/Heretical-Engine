using System.Numerics;

namespace HereticalSolutions.GameEntities
{
    public interface IEventEntityBuilder<TEntity>
    {
        IEventEntityBuilder<TEntity> NewEvent(out TEntity eventEntity);

        IEventEntityBuilder<TEntity> HappenedAtPosition(
            TEntity eventEntity,
            Vector3 position);

        IEventEntityBuilder<TEntity> CausedByEntity(
            TEntity eventEntity,
            Guid sourceEntity);

        IEventEntityBuilder<TEntity> TargetedAtEntity(
            TEntity eventEntity,
            Guid targetEntity);

        IEventEntityBuilder<TEntity> TargetedAtPosition(
            TEntity eventEntity,
            Vector3 position);

        IEventEntityBuilder<TEntity> HappenedAtTime(
            TEntity eventEntity,
            long ticks);

        IEventEntityBuilder<TEntity> WithData<TData>(
            TEntity eventEntity,
            TData data);
    }
}