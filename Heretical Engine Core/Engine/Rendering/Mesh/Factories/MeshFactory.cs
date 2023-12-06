using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class MeshFactory
	{
		public static MeshOpenGLStorageHandle BuildMeshOpenGLStorageHandle(
			string meshAssetDescriptorResourcePath,
			string meshAssetDescriptorResourceVariantID,
			ApplicationContext context)
		{
			return new MeshOpenGLStorageHandle(
				meshAssetDescriptorResourcePath,
				meshAssetDescriptorResourceVariantID,
				context);
		}

		public static ConcurrentMeshOpenGLStorageHandle BuildConcurrentMeshOpenGLStorageHandle(
			string meshAssetDescriptorResourcePath,
			string meshAssetDescriptorResourceVariantID,
			ApplicationContext context)
		{
			return new ConcurrentMeshOpenGLStorageHandle(
				meshAssetDescriptorResourcePath,
				meshAssetDescriptorResourceVariantID,
				new SemaphoreSlim(1, 1),
				context);
		}
	}
}