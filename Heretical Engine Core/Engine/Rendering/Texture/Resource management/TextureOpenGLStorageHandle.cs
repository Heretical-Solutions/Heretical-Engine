using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly TextureRAMStorageHandle textureRAMStorageHandle = null;

		private readonly TextureType textureType;

		private readonly GL cachedGL = default;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private TextureOpenGL texture = null;

		public TextureOpenGLStorageHandle(
			TextureRAMStorageHandle textureRAMStorageHandle,
			TextureType textureType,
			GL gl,
			IFormatLogger logger)
		{
			this.textureRAMStorageHandle = textureRAMStorageHandle;

			this.textureType = textureType;

			cachedGL = gl;

			this.logger = logger;


			texture = null;

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

			bool result = await LoadTexture(
				textureRAMStorageHandle,
				textureType,
				cachedGL,
				true,
				progress)
				.ThrowExceptions<bool, TextureOpenGLStorageHandle>(logger);

			if (!result)
			{
				progress?.Report(1f);

				return;
			}

			allocated = true;

			progress?.Report(1f);
		}

		private async Task<bool> LoadTexture(
			TextureRAMStorageHandle textureRAMStorageHandle,
			TextureType textureType,
			GL gl,
			bool freeRamAfterAllocation = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!textureRAMStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0f,
					0.5f);

				await textureRAMStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<TextureOpenGLStorageHandle>(logger);
			}

			var ramTexture = textureRAMStorageHandle.GetResource<Image<Rgba32>>();

			texture = TextureFactory.BuildTextureOpenGL(
				gl,
				ramTexture,
				textureType);

			progress?.Report(0.5f);

			if (freeRamAfterAllocation)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0.5f,
					1f);

				await textureRAMStorageHandle
					.Free(
						localProgress)
					.ThrowExceptions<TextureOpenGLStorageHandle>(logger);
			}

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

			cachedGL.DeleteTexture(texture.Handle);

			texture = null;


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

				return texture;
			}
		}

		public TValue GetResource<TValue>()
		{
			if (!allocated)
				throw new InvalidOperationException("Resource is not allocated.");

			return (TValue)(object)texture; //DO NOT REPEAT
		}

		#endregion
	}
}