namespace HereticalSolutions.GameEntities
{
    public interface IPrototypesRepository<TWorld, TEntity>
    {
        TWorld PrototypeWorld { get; }

        bool HasPrototype(string prototypeID);

        bool TryGetPrototype(
            string prototypeID,
            out TEntity prototypeEntity);

        bool TryAllocatePrototype(
            string prototypeID,
            out TEntity prototypeEntity);

        void RemovePrototype(string prototypeID);

        IEnumerable<string> AllPrototypeIDs { get; }
    }
}