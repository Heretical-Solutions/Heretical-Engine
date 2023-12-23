namespace HereticalSolutions.GameEntities
{
	public interface IPrototypeCompliantWorldController<TWorld, TEntity>
	{
		IPrototypesRepository<TWorld, TEntity> PrototypeRepository { get; }


		bool TrySpawnEntityFromPrototype(
			string prototypeID,
			out TEntity entity);

		bool TrySpawnAndResolveEntityFromPrototype(
			string prototypeID,
			object source,
			out TEntity entity);
	}
}