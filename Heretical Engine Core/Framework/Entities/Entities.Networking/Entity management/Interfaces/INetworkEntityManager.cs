namespace HereticalSolutions.GameEntities
{
	/// <summary>
	/// Interface for managing game entities.
	/// </summary>
	public interface INetworkEntityManager
	{
		void SpawnEntityFromServer(
			Guid guid,
			string prototypeID);
	}
}