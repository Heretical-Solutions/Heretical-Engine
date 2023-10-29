using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureOpenGLStorageHandle
		: IResourceStorageHandle
	{
		private readonly TextureRAMStorageHandle textureRAMStorageHandle = null;

		private readonly TextureType textureType;

		private readonly GL cachedGL = default;


		private bool allocated = false;

		private TextureOpenGL texture = null;

		public TextureOpenGLStorageHandle(
			TextureRAMStorageHandle textureRAMStorageHandle,
			TextureType textureType,
			GL gl)
		{
			this.textureRAMStorageHandle = textureRAMStorageHandle;

			this.textureType = textureType;

			cachedGL = gl;


			texture = null;

			allocated = false;
		}

		#region IResourceStorageHandle

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
				progress);

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
				IProgress<float> localProgress = null;

				if (progress != null)
				{
					var localProgressInstance = new Progress<float>();

					localProgressInstance.ProgressChanged += (sender, value) =>
					{
						progress.Report(value * 0.5f);
					};

					localProgress = localProgressInstance;
				}

				await textureRAMStorageHandle.Allocate(
					localProgress);
			}

			var ramTexture = textureRAMStorageHandle.GetResource<Image<Rgba32>>();

			var handle = LoadTexture(
				ramTexture,
				gl);

			texture = new TextureOpenGL(
				handle,
				textureType);

			progress?.Report(0.5f);

			if (freeRamAfterAllocation)
			{
				IProgress<float> localProgress = null;

				if (progress != null)
				{
					var localProgressInstance = new Progress<float>();

					localProgressInstance.ProgressChanged += (sender, value) =>
					{
						progress.Report(value * 0.5f + 0.5f);
					};

					localProgress = localProgressInstance;
				}

				await textureRAMStorageHandle.Free(
					localProgress);
			}

			progress?.Report(1f);

			return true;
		}

		private unsafe uint LoadTexture(
			Image<Rgba32> ramTexture,
			GL gl)
		{
			var handle = gl.GenTexture();

			Bind(
				gl,
				handle);

			gl.TexImage2D(
				TextureTarget.Texture2D,
				0,
				InternalFormat.Rgba8,
				(uint)ramTexture.Width,
				(uint)ramTexture.Height,
				0,
				PixelFormat.Rgba,
				PixelType.UnsignedByte,
				null);

			ramTexture.ProcessPixelRows(accessor =>
			{
				for (int y = 0; y < accessor.Height; y++)
				{
					fixed (void* data = accessor.GetRowSpan(y))
					{
						gl.TexSubImage2D(
							TextureTarget.Texture2D,
							0,
							0,
							y,
							(uint)accessor.Width,
							1,
							PixelFormat.Rgba,
							PixelType.UnsignedByte,
							data);
					}
				}
			});

			SetParameters(gl);

			return handle;
		}

		private unsafe uint LoadTexture(
			Span<byte> data,
			uint width,
			uint height,
			GL gl)
		{
			var handle = gl.GenTexture();

			Bind(
				gl,
				handle);

			fixed (void* d = &data[0])
			{
				gl.TexImage2D(
					TextureTarget.Texture2D,
					0,
					(int)InternalFormat.Rgba,
					width,
					height,
					0,
					PixelFormat.Rgba,
					PixelType.UnsignedByte,
					d);

				SetParameters(gl);
			}

			return handle;
		}

		private void SetParameters(
			GL gl)
		{
			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureWrapS,
				(int)GLEnum.ClampToEdge);

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureWrapT,
				(int)GLEnum.ClampToEdge);

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureMinFilter,
				(int)GLEnum.LinearMipmapLinear);

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureMagFilter,
				(int)GLEnum.Linear);

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureBaseLevel,
				0);

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureMaxLevel,
				8);

			gl.GenerateMipmap(
				TextureTarget.Texture2D);
		}

		public void Bind(
			GL gl,
			uint handle,
			TextureUnit textureSlot = TextureUnit.Texture0)
		{
			gl.ActiveTexture(textureSlot);

			gl.BindTexture(
				TextureTarget.Texture2D,
				handle);
		}

		public object RawResource
		{
			get => texture;
		}

		public TValue GetResource<TValue>()
		{
			return (TValue)(object)texture;
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
	}
}