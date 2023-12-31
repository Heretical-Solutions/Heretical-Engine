using HereticalSolutions.Repositories;

using World = DefaultEcs.World;

using Entity = DefaultEcs.Entity;

namespace HereticalSolutions.GameEntities
{
	public class PrototypesRepository
		: IPrototypesRepository<World, Entity>
	{
		private readonly IRepository<string, Entity> prototypesRepository;

		public PrototypesRepository(
			World prototypeWorld,
			IRepository<string, Entity> prototypesRepository)
		{
			PrototypeWorld = prototypeWorld;

			this.prototypesRepository = prototypesRepository;
		}

		#region IPrototypesRepository

		public World PrototypeWorld { get; private set; }

		public bool HasPrototype(string prototypeID)
		{
			return prototypesRepository.Has(prototypeID);
		}

		public bool TryGetPrototype(
			string prototypeID,
			out Entity prototypeEntity)
		{
			return prototypesRepository.TryGet(
				prototypeID,
				out prototypeEntity);
		}

		public bool TryAllocatePrototype(
			string prototypeID,
			out Entity prototypeEntity)
		{
			prototypeEntity = PrototypeWorld.CreateEntity();

			if (!prototypesRepository.TryAdd(
				prototypeID,
				prototypeEntity))
			{
				prototypeEntity.Dispose();

				return false;
			}

			return true;
		}

		public void RemovePrototype(string prototypeID)
		{
			prototypesRepository.Remove(prototypeID);
		}

		public IEnumerable<string> AllPrototypeIDs { get => prototypesRepository.Keys; }

		#endregion
	}
}