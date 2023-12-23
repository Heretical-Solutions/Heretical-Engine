using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.GameEntities
{
	public class DisposeProcessedNetworkEventsSystem : AEntitySetSystem<float>
	{
		public DisposeProcessedNetworkEventsSystem(World eventWorld)
			: base(
				eventWorld
					.GetEntities()
					.With<EventProcessedComponent>()
					.AsSet())
		{
		}

		protected override void Update(float deltaTime, in Entity entity)
		{
			if (entity.Has<NotifyPlayersComponent>())
				return;

			if (entity.Has<NotifyHostComponent>())
				return;

			entity.Set<DespawnComponent>();
		}
	}
}