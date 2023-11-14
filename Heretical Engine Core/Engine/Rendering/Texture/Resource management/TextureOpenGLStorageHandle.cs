using System;
using System.Threading.Tasks;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly IReadOnlyResourceStorageHandle textureRAMStorageHandle = null;

		private readonly TextureType textureType;

		private readonly GL cachedGL = default;

		private readonly ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private TextureOpenGL texture = null;

		public TextureOpenGLStorageHandle(
			IReadOnlyResourceStorageHandle textureRAMStorageHandle,
			TextureType textureType,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			this.textureRAMStorageHandle = textureRAMStorageHandle;

			this.textureType = textureType;

			cachedGL = gl;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;

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

			logger?.Log<TextureOpenGLStorageHandle>(
				$"ALLOCATING");

			Func<Task> loadTextureDelegate = async () =>
			{
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
			};

			var command = new MainThreadCommand(loadTextureDelegate);

			while (!mainThreadCommandBuffer.TryProduce(command))
			{
				await Task.Yield();
			}

			while (command.Status != ECommandStatus.DONE)
			{
				await Task.Yield();
			}

			allocated = true;

			logger?.Log<TextureOpenGLStorageHandle>(
				$"ALLOCATED");

			progress?.Report(1f);
		}

		private async Task<bool> LoadTexture(
			IReadOnlyResourceStorageHandle textureRAMStorageHandle,
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

			logger?.Log<TextureOpenGLStorageHandle>(
				$"FREEING");

			//cachedGL.DeleteTexture(texture.Handle);

			Action deleteShaderDelegate = () =>
			{
				cachedGL.DeleteTexture(texture.Handle);
			};

			var command = new MainThreadCommand(
				deleteShaderDelegate);

			while (!mainThreadCommandBuffer.TryProduce(
				command))
			{
				await Task.Yield();
			}

			while (command.Status != ECommandStatus.DONE)
			{
				await Task.Yield();
			}

			texture = null;


			allocated = false;

			logger?.Log<TextureOpenGLStorageHandle>(
				$"FREE");

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