using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class ModelFactory
	{
		public static ModelOpenGLStorageHandle BuildModelOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle modelRAMStorageHandle,
			IFormatLogger logger)
		{
			return new ModelOpenGLStorageHandle(
				resourceManager,
				modelRAMStorageHandle,
				logger);
		}

		public static ConcurrentModelOpenGLStorageHandle BuildConcurrentModelOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle modelRAMStorageHandle,
			IFormatLogger logger)
		{
			return new ConcurrentModelOpenGLStorageHandle(
				resourceManager,
				modelRAMStorageHandle,
				new SemaphoreSlim(1, 1),
				logger);
		}
	}
}