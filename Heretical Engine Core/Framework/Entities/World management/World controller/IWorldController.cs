namespace HereticalSolutions.GameEntities
{
    public interface IWorldController<TWorld, TSystem, TEntity>
    {
        TWorld World { get; }


        //Entity systems
        TSystem EntityResolveSystems { get; }

        TSystem EntityInitializationSystems { get; }

        TSystem EntityDeinitializationSystems { get; }


        bool TrySpawnEntity(
            out TEntity entity);

        bool TrySpawnAndResolveEntity(
            object source,
            out TEntity entity);

        void DespawnEntity(
            TEntity entity);
    }
}