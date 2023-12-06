#define PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.AssetImport;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshOpenGLStorageHandle
		: AReadOnlyResourceStorageHandle<MeshOpenGL>
	{
		private readonly string meshPrototypeDescriptorResourcePath;

		private readonly string meshPrototypeDescriptorResourceVariantID;

		public MeshOpenGLStorageHandle(
			string meshPrototypeDescriptorResourcePath,
			string meshPrototypeDescriptorResourceVariantID,
			ApplicationContext context)
			: base(
				context)
		{
			this.meshPrototypeDescriptorResourcePath = meshPrototypeDescriptorResourcePath;

			this.meshPrototypeDescriptorResourceVariantID = meshPrototypeDescriptorResourceVariantID;
		}

		protected override async Task<MeshOpenGL> AllocateResource(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			IReadOnlyResourceStorageHandle meshPrototypeResourceStorageHandle = null;

			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				0.333f);

			meshPrototypeResourceStorageHandle = await LoadDependency(
				meshPrototypeDescriptorResourcePath,
				meshPrototypeDescriptorResourceVariantID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, MeshOpenGLStorageHandle>(context.Logger);

			var meshPrototypeDescriptor = meshPrototypeResourceStorageHandle.GetResource<MeshPrototypeDescriptor>();

			IReadOnlyResourceStorageHandle geometryOpenGLStorageHandle = null;

			IReadOnlyResourceStorageHandle materialOpenGLStorageHandle = null;

			progress?.Report(0.333f);

#if PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES
			List<Task> loadDependencyTasks = new List<Task>();

			List<float> loadDependencyProgresses = new List<float>();

			IProgress<float> geometryOpenGLLoadProgress = progress.CreateLocalProgress(
				0.333f,
				1f,
				loadDependencyProgresses,
				0);

			Func<Task> loadGeometryOpenGL = async () =>
			{
				geometryOpenGLStorageHandle = await LoadDependency(
					meshPrototypeDescriptor.GeometryResourcePath,
					AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID,
					geometryOpenGLLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, MeshOpenGLStorageHandle>(context.Logger);
			};

			loadDependencyTasks.Add(loadGeometryOpenGL());

			IProgress<float> materialOpenGLLoadProgress = progress.CreateLocalProgress(
				0.333f,
				1f,
				loadDependencyProgresses,
				1);

			Func<Task> loadMaterialOpenGL = async () =>
			{
				materialOpenGLStorageHandle = await LoadDependency(
					meshPrototypeDescriptor.MaterialResourcePath,
					AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID,
					materialOpenGLLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, MeshOpenGLStorageHandle>(context.Logger);
			};

			loadDependencyTasks.Add(loadMaterialOpenGL());

			await Task
				.WhenAll(loadDependencyTasks)
				.ThrowExceptions<MeshOpenGLStorageHandle>(context.Logger);
#else
			localProgress = progress.CreateLocalProgress(
				0.333f,
				0.666f);

			geometryOpenGLStorageHandle = await LoadDependency(
				meshPrototypeDescriptor.GeometryResourcePath,
				AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, MeshOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.666f);

			localProgress = progress.CreateLocalProgress(
				0.666f,
				1f);

			materialOpenGLStorageHandle = await LoadDependency(
				meshPrototypeDescriptor.MaterialResourcePath,
				AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, MeshOpenGLStorageHandle>(context.Logger);
#endif

			var geometry = geometryOpenGLStorageHandle.GetResource<GeometryOpenGL>();

			var material = materialOpenGLStorageHandle.GetResource<MaterialOpenGL>();

			var mesh = new MeshOpenGL(
				geometry,
				material);

			progress?.Report(1f);

			return mesh;
		}

		protected override async Task FreeResource(
			MeshOpenGL resource,
			IProgress<float> progress = null)
		{
			progress?.Report(1f);
		}
	}
}