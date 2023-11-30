using Silk.NET.Assimp;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureOpenGL
	{
		//Courtesy of https://gamedev.stackexchange.com/questions/140789/texture-coordinates-seem-to-have-flipped-or-incorrect-position
		private const bool FLIP_V_COORDINATES = true;

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
			Image<Rgba32> ramTexture)
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
							(FLIP_V_COORDINATES
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

			SetParameters(gl);
		}

		//WARNING! THIS ONE WOULD PROBABLY NOT FLIP THE TEXTURE VERTICALLY
		public unsafe void Update(
			GL gl,
			Span<byte> data,
			uint width,
			uint height)
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

				SetParameters(gl);
			}
		}

		public void SetParameters(
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
	}
}