namespace HereticalSolutions.GameEntities
{
	public interface IReadOnlyEntityWorldsRepository<TWorld, TSystem, TEntity>
	{
		TWorld GetWorld(string worldID);

		IWorldController<TWorld, TSystem, TEntity> GetWorldController(string worldID);

		IWorldController<TWorld, TSystem, TEntity> GetWorldController(TWorld world);
	}
}