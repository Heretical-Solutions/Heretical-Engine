using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class MeshFactory
	{
		public static MeshOpenGLStorageHandle BuildMeshOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle meshRAMStorageHandle,
			IFormatLogger logger)
		{
			return new MeshOpenGLStorageHandle(
				resourceManager,
				meshRAMStorageHandle,
				logger);
		}

		public static ConcurrentMeshOpenGLStorageHandle BuildConcurrentMeshOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle meshRAMStorageHandle,
			IFormatLogger logger)
		{
			return new ConcurrentMeshOpenGLStorageHandle(
				resourceManager,
				meshRAMStorageHandle,
				new SemaphoreSlim(1, 1),
				logger);
		}
	}
}