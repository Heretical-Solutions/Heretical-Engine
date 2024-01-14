using System.Numerics;

using Silk.NET.OpenGL;

using Silk.NET.Maths;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public struct ShaderOpenGL
	{
		public uint Handle;

		public ShaderDescriptorOpenGL Descriptor { get; private set; }

		public ShaderOpenGL(
			uint handle,
			ShaderDescriptorOpenGL descriptor)
		{
			Handle = handle;

			Descriptor = descriptor;
		}

		public void Use(
			GL gl)
		{
			gl.UseProgram(Handle);
		}

		public void SetUniform(
			GL gl,
			string name,
			int value)
		{
			int location = gl.GetUniformLocation(
				Handle,
				name);

			if (location == -1)
			{
				throw new Exception($"{name} uniform not found on shader.");
			}

			gl.Uniform1(
				location,
				value);
		}

		public void SetUniform(
			GL gl,
			string name,
			Vector3D<float> value)
		{
			int location = gl.GetUniformLocation(
				Handle,
				name);

			if (location == -1)
			{
				throw new Exception($"{name} uniform not found on shader.");
			}

			gl.Uniform3(
				location,
				value.X,
				value.Y,
				value.Z);
		}

		public void SetUniform(
			GL gl,
			string name,
			Vector3 value)
		{
			int location = gl.GetUniformLocation(
				Handle,
				name);

			if (location == -1)
			{
				throw new Exception($"{name} uniform not found on shader.");
			}

			gl.Uniform3(
				location,
				value.X,
				value.Y,
				value.Z);
		}

		public unsafe void SetUniform(
			GL gl,
			string name,
			Matrix4x4 value)
		{
			//A new overload has been created for setting a uniform so we can use the transform in our shader.
			int location = gl.GetUniformLocation(
				Handle,
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
			int location = gl.GetUniformLocation(
				Handle,
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

		public void SetUniform(
			GL gl,
			string name,
			float value)
		{
			int location = gl.GetUniformLocation(
				Handle,
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