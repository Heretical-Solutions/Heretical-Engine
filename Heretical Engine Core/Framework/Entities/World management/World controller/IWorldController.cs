namespace HereticalSolutions.GameEntities
{
    public interface IWorldController<TWorld, TSystem, TEntity>
    {
        //World
        TWorld World { get; }


        //Prototypes
        IPrototypesRepository<TEntity> PrototypeRepository { get; }


        //Entity systems
        TSystem EntityResolveSystems { get; }

        TSystem EntityInitializationSystems { get; }

        TSystem EntityDeinitializationSystems { get; }


        bool TrySpawnEntityFromPrototype(
            string prototypeID,
            out TEntity entity);

        bool TrySpawnAndResolveEntityFromPrototype(
            string prototypeID,
            object source,
            out TEntity entity);

        void DespawnEntity(
            TEntity entity);
    }
}