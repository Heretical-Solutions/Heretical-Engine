namespace HereticalSolutions.GameEntities
{
    public interface IReadOnlyEntityRepository<TEntity>
    {
        bool HasEntity(Guid guid);
        
        TEntity GetRegistryEntity(Guid guid);

        GuidPrototypeIDPair[] AllRegistryEntities { get; }
        
        IEnumerable<Guid> AllAllocatedGUIDs { get; }
    }
}