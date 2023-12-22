namespace HereticalSolutions.GameEntities
{
	public interface IGUIDCompliantWorldController<TEntity>
	{
		bool TrySpawnEntityWithGUIDFromPrototype(
			string prototypeID,
			Guid guid,
			out TEntity entity);

		bool TrySpawnAndResolveEntityWithGUIDFromPrototype(
			string prototypeID,
			Guid guid,
			object source,
			out TEntity entity);
	}
}