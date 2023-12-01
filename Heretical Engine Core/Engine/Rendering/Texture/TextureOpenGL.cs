using Silk.NET.Assimp;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureOpenGL
	{
		//Courtesy of https://gamedev.stackexchange.com/questions/140789/texture-coordinates-seem-to-have-flipped-or-incorrect-position
		//and https://www.puredevsoftware.com/blog/2018/03/17/texture-coordinates-d3d-vs-opengl/
		private const bool FLIP_TEXTURE_VERTICALLY = true;

		private uint handle;

		public uint Handle { get => handle; }

		public TextureType Type { get; private set; }

		public TextureOpenGL(
			uint handle,
			TextureType type = TextureType.None)
		{
			this.handle = handle;

			Type = type;
		}

		public void Bind(
			GL gl,
			TextureUnit textureSlot = TextureUnit.Texture0)
		{
			gl.ActiveTexture(textureSlot);

			gl.BindTexture(
				TextureTarget.Texture2D,
				handle);
		}

		public unsafe void Update(
			GL gl,
			Image<Rgba32> ramTexture,
			TextureDescriptorDTO descriptor)
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
				descriptor);
		}

		//WARNING! THIS OVERLOAD WON'T FLIP THE TEXTURE VERTICALLY!
		//MARKING WITH Obsolete TO CAST WARNINGS
		[Obsolete("This overload won't flip the texture vertically")]
		public unsafe void Update(
			GL gl,
			Span<byte> data,
			uint width,
			uint height,
			TextureDescriptorDTO descriptor)
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
					descriptor);
			}
		}

		public void SetParameters(
			GL gl,
			TextureDescriptorDTO descriptor)
		{
			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureWrapS,
				descriptor.WrapS);

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureWrapT,
				descriptor.WrapT);

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureMinFilter,
				descriptor.MinFilter);

			gl.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureMagFilter,
				descriptor.MagFilter);

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