using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MaterialOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly IRuntimeResourceManager resourceManager = null;

		private readonly IReadOnlyResourceStorageHandle materialRAMStorageHandle = null;


		private bool allocated = false;

		private MaterialOpenGL material = null;

		public MaterialOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle materialRAMStorageHandle)
		{
			this.resourceManager = resourceManager;

			this.materialRAMStorageHandle = materialRAMStorageHandle;


			material = null;

			allocated = false;
		}

		#region IReadOnlyResourceStorageHandle

		#region IAllocatable

		public bool Allocated
		{
			get => allocated;
		}

		public virtual async Task Allocate(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (allocated)
			{
				progress?.Report(1f);

				return;
			}

			bool result = await LoadMaterial(
				resourceManager,
				materialRAMStorageHandle,
				progress);

			if (!result)
			{
				progress?.Report(1f);

				return;
			}

			allocated = true;

			progress?.Report(1f);
		}

		private async Task<bool> LoadMaterial(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle materialRAMStorageHandle,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!materialRAMStorageHandle.Allocated)
			{
				IProgress<float> localProgress = null;

				if (progress != null)
				{
					var localProgressInstance = new Progress<float>();

					localProgressInstance.ProgressChanged += (sender, value) =>
					{
						progress.Report(value * 0.333f);
					};

					localProgress = localProgressInstance;
				}

				await materialRAMStorageHandle.Allocate(
					localProgress);
			}

			var materialDTO = materialRAMStorageHandle.GetResource<MaterialDTO>();

			progress?.Report(0.333f);

			var shaderStorageHandle = resourceManager
				.GetResource(
					materialDTO.ShaderResourceID.SplitAddressBySeparator())
				.GetVariant(
					ShaderOpenGLAssetImporter.SHADER_OPENGL_VARIANT_ID)
				.StorageHandle;

			if (!shaderStorageHandle.Allocated)
			{
				IProgress<float> localProgress = null;

				if (progress != null)
				{
					var localProgressInstance = new Progress<float>();

					localProgressInstance.ProgressChanged += (sender, value) =>
					{
						progress.Report(value * 0.333f + 0.333f);
					};

					localProgress = localProgressInstance;
				}

				await shaderStorageHandle.Allocate(
					localProgress);
			}

			var shader = shaderStorageHandle.GetResource<ShaderOpenGL>();

			progress?.Report(0.666f);

			var textures = new TextureOpenGL[materialDTO.TextureResourceIDs.Length];

			for (int i = 0; i < textures.Length; i++)
			{
				var textureOpenGLStorageHandle = resourceManager
					.GetResource(
						materialDTO.TextureResourceIDs[i].SplitAddressBySeparator())
					.GetVariant(
						TextureOpenGLAssetImporter.TEXTURE_OPENGL_VARIANT_ID)
					.StorageHandle;

				if (!textureOpenGLStorageHandle.Allocated)
				{
					IProgress<float> localProgress = null;

					if (progress != null)
					{
						var localProgressInstance = new Progress<float>();

						localProgressInstance.ProgressChanged += (sender, value) =>
						{
							progress.Report(0.333f * ((value + (float)i) / (float)textures.Length) + 0.666f);
						};

						localProgress = localProgressInstance;
					}

					await textureOpenGLStorageHandle.Allocate(
						localProgress);
				}

				textures[i] = textureOpenGLStorageHandle.GetResource<TextureOpenGL>();
			}

			material = new MaterialOpenGL(
				shader,
				textures);

			progress?.Report(1f);

			return true;
		}

		public virtual async Task Free(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!allocated)
			{
				progress?.Report(1f);

				return;
			}

			material = null;


			allocated = false;

			progress?.Report(1f);
		}

		#endregion

		public object RawResource
		{
			get
			{
				if (!allocated)
					throw new InvalidOperationException("Resource is not allocated.");

				return material;
			}
		}

		public TValue GetResource<TValue>()
		{
			if (!allocated)
				throw new InvalidOperationException("Resource is not allocated.");

			return (TValue)(object)material; //DO NOT REPEAT
		}

		#endregion
	}
}