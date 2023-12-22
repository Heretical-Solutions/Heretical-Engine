using HereticalSolutions.Repositories;

using Entity = DefaultEcs.Entity;

namespace HereticalSolutions.GameEntities
{
	public class PrototypesRepository
		: IPrototypesRepository<Entity>
	{
		private readonly IRepository<string, Entity> prototypesRepository;

		public PrototypesRepository(
			IRepository<string, Entity> prototypesRepository)
		{
			this.prototypesRepository = prototypesRepository;
		}

		#region IPrototypesRepository

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

		public void AddPrototype(
			string prototypeID,
			Entity prototypeEntity)
		{
			prototypesRepository.TryAdd(
				prototypeID,
				prototypeEntity);
		}

		public void RemovePrototype(string prototypeID)
		{
			prototypesRepository.Remove(prototypeID);
		}

		#endregion
	}
}