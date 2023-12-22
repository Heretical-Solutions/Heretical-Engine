using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
	/// <summary>
	/// Represents a class for building event entities.
	/// </summary>
	public class NetworkEventEntityBuilder
		: EventEntityBuilder,
		  INetworkEventEntityBuilder<Entity>
	{
		public NetworkEventEntityBuilder(
			World eventWorld)
			: base (eventWorld)
		{
		}

        public IEventEntityBuilder<Entity> HostShouldBeNotified(Entity eventEntity)
        {
            eventEntity.Set<NotifyHostComponent>(new NotifyHostComponent());
            
            return this;
        }

        public IEventEntityBuilder<Entity> PlayersShouldBeNotified(Entity eventEntity)
        {
            eventEntity.Set<NotifyPlayersComponent>(new NotifyPlayersComponent());
            
            return this;
        }
	}
}