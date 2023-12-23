namespace HereticalSolutions.GameEntities
{
	public static partial class WorldConstants
	{
		public const string REGISTRY_WORLD_ID = "Registry world";

		//Now that we have a prototype world for each of the worlds we won't be searching for it by an ID. We'll be taking it from the respective world controller's prototype repository instead
		//public const string PROTOTYPE_WORLD_ID = "Prototype world";

		public const string SIMULATION_WORLD_ID = "Simulation world";

		public const string VIEW_WORLD_ID = "View world";

		//TODO: introduce several event worlds
		public const string EVENT_WORLD_ID = "Event world";
	}
}