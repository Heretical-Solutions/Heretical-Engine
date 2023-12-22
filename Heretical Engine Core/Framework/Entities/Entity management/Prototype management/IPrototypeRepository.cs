namespace HereticalSolutions.GameEntities
{
    public interface IPrototypesRepository<TEntity>
    {
        bool HasPrototype(string prototypeID);

        bool TryGetPrototype(
            string prototypeID,
            out TEntity prototypeEntity);

        void AddPrototype(
            string prototypeID,
            TEntity prototypeEntity);

        void RemovePrototype(string prototypeID);
    }
}