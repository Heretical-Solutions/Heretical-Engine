#define PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MaterialOpenGLStorageHandle
		: AReadOnlyResourceStorageHandle<MaterialOpenGL>
	{
		private readonly string materialRAMPath;

		private readonly string materialRAMVariantID;

		public MaterialOpenGLStorageHandle(
			string materialRAMPath,
			string materialRAMVariantID,
			ApplicationContext context)
			: base (
				context)
		{
			this.materialRAMPath = materialRAMPath;

			this.materialRAMVariantID = materialRAMVariantID;
		}

		protected override async Task<MaterialOpenGL> AllocateResource(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);


			IReadOnlyResourceStorageHandle materialRAMStorageHandle = null;

			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				0.333f);

			materialRAMStorageHandle = await LoadDependency(
				materialRAMPath,
				materialRAMVariantID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, MaterialOpenGLStorageHandle>(context.Logger);

			var materialDTO = materialRAMStorageHandle.GetResource<MaterialDTO>();

			IReadOnlyResourceStorageHandle shaderOpenGLStorageHandle = null;

			IReadOnlyResourceStorageHandle[] textureOpenGLStorageHandles = new IReadOnlyResourceStorageHandle[materialDTO.TextureResourceIDs.Length];

			progress?.Report(0.333f);


#if PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES
			List<Task> loadDependencyTasks = new List<Task>();

			List<float> loadDependencyProgresses = new List<float>();

			IProgress<float> shaderLoadProgress = progress.CreateLocalProgress(
				0.333f,
				1f,
				loadDependencyProgresses,
				0);

			loadDependencyTasks.Add(
				Task.Run(async () => 
					{
						shaderOpenGLStorageHandle = await LoadDependency(
							materialDTO.ShaderResourceID,
							ShaderOpenGLAssetImporter.SHADER_OPENGL_VARIANT_ID,
							shaderLoadProgress)
							.ThrowExceptions<IReadOnlyResourceStorageHandle, MaterialOpenGLStorageHandle>(context.Logger);
					}));

			for (int i = 0; i < textureOpenGLStorageHandles.Length; i++)
			{
				IProgress<float> textureLoadProgress = progress.CreateLocalProgress(
					0.333f,
					1f,
					loadDependencyProgresses,
					i + 1);

				loadDependencyTasks.Add(
					Task.Run(async () =>
						{
							textureOpenGLStorageHandles[i] = await LoadDependency(
								materialDTO.TextureResourceIDs[i],
								TextureOpenGLAssetImporter.TEXTURE_OPENGL_VARIANT_ID,
								textureLoadProgress)
								.ThrowExceptions<IReadOnlyResourceStorageHandle, MaterialOpenGLStorageHandle>(context.Logger);
						}));
			}

			await Task
				.WhenAll(loadDependencyTasks)
				.ThrowExceptions<MaterialOpenGLStorageHandle>(context.Logger);
#else
			localProgress = progress.CreateLocalProgress(
				0.333f,
				0.666f);

			shaderOpenGLStorageHandle = await LoadDependency(
				materialDTO.ShaderResourceID,
				ShaderOpenGLAssetImporter.SHADER_OPENGL_VARIANT_ID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, MaterialOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.666f);

			for (int i = 0; i < textureOpenGLStorageHandles.Length; i++)
			{
				IProgress<float> textureLoadProgress = progress.CreateLocalProgress(
					0.666f,
					1f,
					i,
					textureOpenGLStorageHandles.Length);

				textureOpenGLStorageHandles[i] = await LoadDependency(
					materialDTO.TextureResourceIDs[i],
					TextureOpenGLAssetImporter.TEXTURE_OPENGL_VARIANT_ID,
					textureLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, MaterialOpenGLStorageHandle>(context.Logger);
			}
#endif

			var shader = shaderOpenGLStorageHandle.GetResource<ShaderOpenGL>();

			var textures = new TextureOpenGL[materialDTO.TextureResourceIDs.Length];

			for (int i = 0; i < textures.Length; i++)
			{
				textures[i] = textureOpenGLStorageHandles[i].GetResource<TextureOpenGL>();
			}

			var material = new MaterialOpenGL(
				shader,
				textures);

			progress?.Report(1f);

			return material;
		}

		protected override async Task FreeResource(
			MaterialOpenGL resource,
			IProgress<float> progress = null)
		{
			progress?.Report(1f);
		}
	}
}