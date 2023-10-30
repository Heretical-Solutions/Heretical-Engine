using System.Numerics;

using Silk.NET.Maths;

namespace HereticalSolutions.HereticalEngine.Math
{
	public static class Matrix4X4Extensions
	{
		//Courtesy of OpenTK https://github.com/opentk/opentk/blob/master/src/OpenTK.Mathematics/Matrix/Matrix4.cs
		public static Matrix4X4<T> Zero<T>()
			where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
		{
			return new Matrix4X4<T>(
				Scalar<T>.Zero, Scalar<T>.Zero, Scalar<T>.Zero, Scalar<T>.Zero,
				Scalar<T>.Zero, Scalar<T>.Zero, Scalar<T>.Zero, Scalar<T>.Zero,
				Scalar<T>.Zero, Scalar<T>.Zero, Scalar<T>.Zero, Scalar<T>.Zero,
				Scalar<T>.Zero, Scalar<T>.Zero, Scalar<T>.Zero, Scalar<T>.Zero);
		}

		public static Matrix4X4<float> ToSilkNetMatrix4X4(this Matrix4x4 matrix)
		{
			return new Matrix4X4<float>(matrix.M11, matrix.M12, matrix.M13, matrix.M14,
				matrix.M21, matrix.M22, matrix.M23, matrix.M24,
				matrix.M31, matrix.M32, matrix.M33, matrix.M34,
				matrix.M41, matrix.M42, matrix.M43, matrix.M44);
		}

		public static Matrix4x4 ToNumericsMatrix4x4(this Matrix4X4<float> matrix)
		{
			return new Matrix4x4(matrix.M11, matrix.M12, matrix.M13, matrix.M14,
				matrix.M21, matrix.M22, matrix.M23, matrix.M24,
				matrix.M31, matrix.M32, matrix.M33, matrix.M34,
				matrix.M41, matrix.M42, matrix.M43, matrix.M44);
		}
	}
}