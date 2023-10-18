using HereticalSolutions.ResourceManagement;

namespace HereticalSolutions.HereticalEngine.Assimp
{
	public abstract class AssetImporter
	{
		protected readonly IRuntimeResourceManager resourceManager;

		public AssetImporter(IRuntimeResourceManager resourceManager)
		{
			this.resourceManager = resourceManager;
		}

		public abstract object Import();
	}
}