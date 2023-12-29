namespace HereticalSolutions.GameEntities
{
	public interface IEntityListManager<TListID, TEntityList>
	{
		bool HasList(TListID listID);

		TEntityList GetList(TListID listID);

		void CreateList(
			out TListID listID,
			out TEntityList entityList);

		void RemoveList(TListID listID);
	}
}