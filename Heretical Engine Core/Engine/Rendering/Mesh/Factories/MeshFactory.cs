using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class MeshFactory
	{
		public static MeshOpenGLStorageHandle BuildMeshOpenGLStorageHandle(
			string meshRAMPath,
			string meshRAMVariantID,
			ApplicationContext context)
		{
			return new MeshOpenGLStorageHandle(
				meshRAMPath,
				meshRAMVariantID,
				context);
		}

		public static ConcurrentMeshOpenGLStorageHandle BuildConcurrentMeshOpenGLStorageHandle(
			string meshRAMPath,
			string meshRAMVariantID,
			ApplicationContext context)
		{
			return new ConcurrentMeshOpenGLStorageHandle(
				meshRAMPath,
				meshRAMVariantID,
				new SemaphoreSlim(1, 1),
				context);
		}
	}
}