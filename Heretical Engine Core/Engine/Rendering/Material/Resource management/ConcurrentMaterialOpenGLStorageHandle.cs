using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ConcurrentMaterialOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly IRuntimeResourceManager resourceManager = null;

		private readonly IReadOnlyResourceStorageHandle materialRAMStorageHandle = null;

		private readonly SemaphoreSlim semaphore;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private MaterialOpenGL material = null;

		public ConcurrentMaterialOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle materialRAMStorageHandle,
			SemaphoreSlim semaphore,
			IFormatLogger logger)
		{
			this.resourceManager = resourceManager;

			this.materialRAMStorageHandle = materialRAMStorageHandle;

			this.logger = logger;

			
			material = null;

			allocated = false;
		}

		#region IReadOnlyResourceStorageHandle

		#region IAllocatable

		public bool Allocated
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return allocated;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public async Task Allocate(IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (allocated)
				{
					progress?.Report(1f);

					return;
				}

				bool result = await LoadMaterial(
					resourceManager,
					materialRAMStorageHandle,
					progress)
					.ThrowExceptions<bool, ConcurrentMaterialOpenGLStorageHandle>(logger);

				if (!result)
				{
					progress?.Report(1f);

					return;
				}

				allocated = true;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
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

				await materialRAMStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<ConcurrentMaterialOpenGLStorageHandle>(logger);
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

				await shaderStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<ConcurrentMaterialOpenGLStorageHandle>(logger);
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

					await textureOpenGLStorageHandle
						.Allocate(
							localProgress)
						.ThrowExceptions<ConcurrentMaterialOpenGLStorageHandle>(logger);
				}

				textures[i] = textureOpenGLStorageHandle.GetResource<TextureOpenGL>();
			}

			material = new MaterialOpenGL(
				shader,
				textures);

			progress?.Report(1f);

			return true;
		}

		/*
		private async Task<bool> LoadMaterial(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle materialRAMStorageHandle,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
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

					await materialRAMStorageHandle
						.Allocate(
							localProgress)
						.ThrowExceptions<ConcurrentMaterialOpenGLStorageHandle>(logger);
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

					await shaderStorageHandle
						.Allocate(
							localProgress)
						.ThrowExceptions<ConcurrentMaterialOpenGLStorageHandle>(logger);
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

						await textureOpenGLStorageHandle
							.Allocate(
								localProgress)
							.ThrowExceptions<ConcurrentMaterialOpenGLStorageHandle>(logger);
					}

					textures[i] = textureOpenGLStorageHandle.GetResource<TextureOpenGL>();
				}

				material = new MaterialOpenGL(shader, textures);

				progress?.Report(1f);

				return true;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}
		*/

		public async Task Free(IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore
			
			try
			{
				if (!allocated)
				{
					progress?.Report(1f);

					return;
				}

				material = null;

				allocated = false;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		#endregion

		public object RawResource
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					if (!allocated)
						throw new InvalidOperationException("Resource is not allocated.");

					return material;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public TValue GetResource<TValue>()
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!allocated)
					throw new InvalidOperationException("Resource is not allocated.");

				return (TValue)(object)material;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		#endregion
	}
}