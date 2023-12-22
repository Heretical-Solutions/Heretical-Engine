namespace HereticalSolutions.GameEntities
{
	public interface IEntityListManager<TEntityList>
	{
		TEntityList GetList(int listID);

		int AddList(TEntityList entityList);

		void RemoveList(int listID);
	}
}