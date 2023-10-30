using HereticalSolutions.ResourceManagement;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class MeshFactory
	{
		public static MeshOpenGLStorageHandle BuildMeshOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle meshRAMStorageHandle)
		{
			return new MeshOpenGLStorageHandle(
				resourceManager,
				meshRAMStorageHandle);
		}
	}
}