namespace HereticalSolutions.Synchronization
{
    public struct SynchronizationDescriptor
    {
        public readonly string ID;

        public readonly bool CanBeToggled = true;

        public bool Active = true;

        public readonly bool CanScale = true;

        public float Scale = 1f;

        public SynchronizationDescriptor(
            string id,
            bool canBeToggled,
            bool canScale)
        {
            ID = id;

            CanBeToggled = canBeToggled;

            CanScale = canScale;
        }
    }
}