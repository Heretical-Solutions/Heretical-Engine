/*
using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.AssetImport;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MaterialOpenGLStorageHandle
		: AReadOnlyResourceStorageHandle<MaterialOpenGL>
	{
		private readonly string materialPrototypeDescriptorResourcePath;

		private readonly string materialPrototypeDescriptorResourceVariantID;

		public MaterialOpenGLStorageHandle(
			string materialPrototypeDescriptorResourcePath,
			string materialPrototypeDescriptorResourceVariantID,
			ApplicationContext context)
			: base (
				context)
		{
			this.materialPrototypeDescriptorResourcePath = materialPrototypeDescriptorResourcePath;

			this.materialPrototypeDescriptorResourceVariantID = materialPrototypeDescriptorResourceVariantID;
		}

		protected override async Task<MaterialOpenGL> AllocateResource(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);


			IReadOnlyResourceStorageHandle materialPrototypeDescriptorStorageHandle = null;

			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				0.333f);

			materialPrototypeDescriptorStorageHandle = await LoadDependency(
				materialPrototypeDescriptorResourcePath,
				materialPrototypeDescriptorResourceVariantID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, MaterialOpenGLStorageHandle>(context.Logger);

			var materialPrototypeDescriptor = materialPrototypeDescriptorStorageHandle.GetResource<MaterialPrototypeDescriptor>();

			IReadOnlyResourceStorageHandle shaderOpenGLStorageHandle = null;

			IReadOnlyResourceStorageHandle[] textureOpenGLStorageHandles = new IReadOnlyResourceStorageHandle[materialPrototypeDescriptor.TextureResourcePaths.Length];

			progress?.Report(0.333f);


#if PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES
			List<Task> loadDependencyTasks = new List<Task>();

			List<float> loadDependencyProgresses = new List<float>();

			IProgress<float> shaderLoadProgress = progress.CreateLocalProgress(
				0.333f,
				1f,
				loadDependencyProgresses,
				0);

			Func<Task> loadShaderOpenGL = async () =>
			{
				shaderOpenGLStorageHandle = await LoadDependency(
					materialPrototypeDescriptor.ShaderResourcePath,
					AssetImportConstants.ASSET_SHADER_OPENGL_VARIANT_ID,
					shaderLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, MaterialOpenGLStorageHandle>(context.Logger);
			};

			loadDependencyTasks.Add(loadShaderOpenGL());

			for (int i = 0; i < textureOpenGLStorageHandles.Length; i++)
			{
				IProgress<float> textureLoadProgress = progress.CreateLocalProgress(
					0.333f,
					1f,
					loadDependencyProgresses,
					i + 1);

				int iClosure = i;

				Func<Task> loadTextureOpenGL = async () =>
				{
					textureOpenGLStorageHandles[iClosure] = await LoadDependency(
						materialPrototypeDescriptor.TextureResourcePaths[iClosure],
						AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID,
						textureLoadProgress)
						.ThrowExceptions<IReadOnlyResourceStorageHandle, MaterialOpenGLStorageHandle>(context.Logger);
				};

				loadDependencyTasks.Add(loadTextureOpenGL());
			}

			await Task
				.WhenAll(loadDependencyTasks)
				.ThrowExceptions<MaterialOpenGLStorageHandle>(context.Logger);
#else
			localProgress = progress.CreateLocalProgress(
				0.333f,
				0.666f);

			shaderOpenGLStorageHandle = await LoadDependency(
				materialPrototypeDescriptor.ShaderResourcePath,
				AssetImportConstants.ASSET_SHADER_OPENGL_VARIANT_ID,
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
					materialPrototypeDescriptor.TextureResourcePaths[i],
					AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID,
					textureLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, MaterialOpenGLStorageHandle>(context.Logger);
			}
#endif

			var shader = shaderOpenGLStorageHandle.GetResource<ShaderOpenGL>();

			var textures = new TextureOpenGL[materialPrototypeDescriptor.TextureResourcePaths.Length];

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
*/