using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class MaterialFactory
	{
		public static MaterialOpenGLStorageHandle BuildMaterialOpenGLStorageHandle(
			string materialRAMPath,
			string materialRAMVariantID,
			ApplicationContext context)
		{
			return new MaterialOpenGLStorageHandle(
				materialRAMPath,
				materialRAMVariantID,
				context);
		}

		public static ConcurrentMaterialOpenGLStorageHandle BuildConcurrentMaterialOpenGLStorageHandle(
			string materialRAMPath,
			string materialRAMVariantID,
			ApplicationContext context)
		{
			return new ConcurrentMaterialOpenGLStorageHandle(
				materialRAMPath,
				materialRAMVariantID,
				new SemaphoreSlim(1, 1),
				context);
		}
	}
}