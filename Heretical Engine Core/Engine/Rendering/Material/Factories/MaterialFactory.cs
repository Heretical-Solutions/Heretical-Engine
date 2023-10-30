using HereticalSolutions.ResourceManagement;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class MaterialFactory
	{
		public static MaterialOpenGLStorageHandle BuildMaterialOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle materialRAMStorageHandle)
		{
			return new MaterialOpenGLStorageHandle(
				resourceManager,
				materialRAMStorageHandle);
		}
	}
}