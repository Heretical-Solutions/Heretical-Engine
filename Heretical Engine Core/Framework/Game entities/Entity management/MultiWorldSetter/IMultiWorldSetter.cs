namespace HereticalSolutions.GameEntities
{
    public interface IMultiWorldSetter
    {
        void SetToAllWorld<T>(T component);
    }
}