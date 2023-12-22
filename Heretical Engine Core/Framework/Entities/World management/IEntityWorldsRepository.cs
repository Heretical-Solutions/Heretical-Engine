namespace HereticalSolutions.GameEntities
{
    public interface IEntityWorldsRepository<TWorld, TSystem, TEntity>
        : IReadOnlyEntityWorldsRepository<TWorld, TSystem, TEntity>
    {
        bool HasWorld(string worldID);

        bool HasWorld(TWorld world);


        void AddWorld(
            string worldID,
            IWorldController<TWorld, TSystem, TEntity> worldController);

        
        void RemoveWorld(string worldID);

        void RemoveWorld(TWorld world);
    }
}