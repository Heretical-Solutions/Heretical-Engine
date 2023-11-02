using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class MaterialFactory
	{
		public static MaterialOpenGLStorageHandle BuildMaterialOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle materialRAMStorageHandle,
			IFormatLogger logger)
		{
			return new MaterialOpenGLStorageHandle(
				resourceManager,
				materialRAMStorageHandle,
				logger);
		}

		public static ConcurrentMaterialOpenGLStorageHandle BuildConcurrentMaterialOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle materialRAMStorageHandle,
			IFormatLogger logger)
		{
			return new ConcurrentMaterialOpenGLStorageHandle(
				resourceManager,
				materialRAMStorageHandle,
				new ReaderWriterLockSlim(),
				logger);
		}
	}
}