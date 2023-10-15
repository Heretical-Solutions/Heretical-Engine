using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    public class MultiWorldSetter : IMultiWorldSetter
    {
        private World[] worlds;
        
        public MultiWorldSetter(World[] worlds)
        {
            this.worlds = worlds;
        }


        public void SetToAllWorld<T>(T Component)
        {
            foreach (var world in worlds)
            {
                if(!world.Has<T>())
                    world.Set<T>(Component);
            }
        }
    }
}