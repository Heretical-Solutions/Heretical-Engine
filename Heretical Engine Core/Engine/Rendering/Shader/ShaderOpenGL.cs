using System.Numerics;

using Silk.NET.OpenGL;

using Silk.NET.Maths;
using HereticalSolutions.HereticalEngine.Math;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ShaderOpenGL
	{
		private uint handle = default;

		public uint Handle { get => handle; }

		public ShaderOpenGL(
			uint handle)
		{
			this.handle = handle;
		}

		public void Use(
			GL gl)
		{
			gl.UseProgram(handle);
		}

		public void SetUniform(
			GL gl,
			string name,
			int value)
		{
			int location = gl.GetUniformLocation(
				handle,
				name);

			if (location == -1)
			{
				throw new Exception($"{name} uniform not found on shader.");
			}

			gl.Uniform1(
				location,
				value);
		}

		public unsafe void SetUniform(
			GL gl,
			string name,
			Matrix4x4 value)
		{
			//A new overload has been created for setting a uniform so we can use the transform in our shader.
			int location = gl.GetUniformLocation(
				handle,
				name);
			
			if (location == -1)
			{
				throw new Exception($"{name} uniform not found on shader.");
			}

			gl.UniformMatrix4(
				location,
				1,
				false,
				(float*)&value);
		}

		public unsafe void SetUniform(
			GL gl,
			string name,
			Matrix4X4<float> value)
		{
			SetUniform(
				gl,
				name,
				value.ToNumericsMatrix4x4());
		}

		public void SetUniform(
			GL gl,
			string name,
			float value)
		{
			int location = gl.GetUniformLocation(
				handle,
				name);

			if (location == -1)
			{
				throw new Exception($"{name} uniform not found on shader.");
			}

			gl.Uniform1(
				location,
				value);
		}
	}
}