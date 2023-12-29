using HereticalSolutions.Logging;

using Silk.NET.Assimp;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureOpenGL
	{
		//Courtesy of https://gamedev.stackexchange.com/questions/140789/texture-coordinates-seem-to-have-flipped-or-incorrect-position
		//and https://www.puredevsoftware.com/blog/2018/03/17/texture-coordinates-d3d-vs-opengl/
		private const bool FLIP_TEXTURE_VERTICALLY = true;

		public uint Handle;

		public TextureType Type { get; private set; }

		public TextureOpenGL(
			uint handle,
			TextureType type = TextureType.None)
		{
			Handle = handle;

			Type = type;
		}

		public void Bind(
			GL gl,
			TextureUnit textureSlot = TextureUnit.Texture0)
		{
			gl.ActiveTexture(textureSlot);

			gl.BindTexture(
				TextureTarget.Texture2D,
				Handle);
		}

		public unsafe void Update(
			GL gl,
			Image<Rgba32> ramTexture,
			TextureAssetDescriptor descriptor,
			IFormatLogger logger)
		{
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
							(FLIP_TEXTURE_VERTICALLY
								? accessor.Height - y - 1
								: y),
							(uint)accessor.Width,
							1,
							PixelFormat.Rgba,
							PixelType.UnsignedByte,
							data);
					}
				}
			});

			SetParameters(
				gl,
				descriptor,
				logger);
		}

		//WARNING! THIS OVERLOAD WON'T FLIP THE TEXTURE VERTICALLY!
		//MARKING WITH Obsolete TO CAST WARNINGS
		[Obsolete("This overload won't flip the texture vertically")]
		public unsafe void Update(
			GL gl,
			Span<byte> data,
			uint width,
			uint height,
			TextureAssetDescriptor descriptor,
			IFormatLogger logger)
		{
			fixed (void* dataPointer = &data[0])
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
					dataPointer);

				SetParameters(
					gl,
					descriptor,
					logger);
			}
		}

		public void SetParameters(
			GL gl,
			TextureAssetDescriptor descriptor,
			IFormatLogger logger)
		{
			if (!Enum.TryParse(
				descriptor.WrapS,
				out GLEnum wrapS))
			{
				logger?.ThrowException<TextureOpenGL>(
					$"COULD NOT PARSE WrapS: {descriptor.WrapS}");
			}

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureWrapS,
				(int)wrapS);

			if (!Enum.TryParse(
				descriptor.WrapT,
				out GLEnum wrapT))
			{
				logger?.ThrowException<TextureOpenGL>(
					$"COULD NOT PARSE WrapT: {descriptor.WrapT}");
			}

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureWrapT,
				(int)wrapT);

			if (!Enum.TryParse(
				descriptor.MinFilter,
				out GLEnum minFilter))
			{
				logger?.ThrowException<TextureOpenGL>(
					$"COULD NOT PARSE MinFilter: {descriptor.MinFilter}");
			}

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureMinFilter,
				(int)minFilter);

			if (!Enum.TryParse(
				descriptor.MagFilter,
				out GLEnum magFilter))
			{
				logger?.ThrowException<TextureOpenGL>(
					$"COULD NOT PARSE MinFilter: {descriptor.MagFilter}");
			}

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureMagFilter,
				(int)magFilter);

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureBaseLevel,
				descriptor.BaseLevel);

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureMaxLevel,
				descriptor.MaxLevel);

			gl.GenerateMipmap(
				TextureTarget.Texture2D);
		}
	}
}