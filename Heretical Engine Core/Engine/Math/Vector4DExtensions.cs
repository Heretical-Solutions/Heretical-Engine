using System.Numerics;

using Silk.NET.Maths;

namespace HereticalSolutions.HereticalEngine.Math
{
	//Courtesy of OpenTK https://github.com/opentk/opentk/blob/master/src/OpenTK.Mathematics/Vector/Vector4.cs
	public static class Vector4DExtensions
	{
		public static Vector4D<float> ToSilkNetVector4D(this Vector4 vector)
		{
			return new Vector4D<float>(vector.X, vector.Y, vector.Z, vector.W);
		}

		public static Vector4 ToNumericsVector4(this Vector4D<float> vector)
		{
			return new Vector4(vector.X, vector.Y, vector.Z, vector.W);
		}

		/// <summary>
		/// Transforms a vector by a quaternion rotation.
		/// </summary>
		/// <param name="vec">The vector to transform.</param>
		/// <param name="quat">The quaternion to rotate the vector by.</param>
		/// <returns>The result of the operation.</returns>
		public static Vector4D<T> Transform<T>(
			Vector4D<T> vec,
			Quaternion<T> quat)
			where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
		{
			Transform(
				in vec,
				in quat,
				out Vector4D<T> result);

			return result;
		}

		/// <summary>
		/// Transforms a vector by a quaternion rotation.
		/// </summary>
		/// <param name="vec">The vector to transform.</param>
		/// <param name="quat">The quaternion to rotate the vector by.</param>
		/// <param name="result">The result of the operation.</param>
		public static void Transform<T>(
			in Vector4D<T> vec,
			in Quaternion<T> quat,
			out Vector4D<T> result)
			where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
		{
			Quaternion<T> v = new Quaternion<T>(
				vec.X, vec.Y, vec.Z, vec.W);
				
			var i = Quaternion<T>.Inverse(quat);
			var t = Quaternion<T>.Multiply(quat, v);
			v = Quaternion<T>.Multiply(t, i);

			result.X = v.X;
			result.Y = v.Y;
			result.Z = v.Z;
			result.W = v.W;
		}


		/// <summary>
		/// Transforms a vector by a quaternion rotation.
		/// </summary>
		/// <param name="quat">The quaternion to rotate the vector by.</param>
		/// <param name="vec">The vector to transform.</param>
		/// <returns>The transformed vector.</returns>
		public static Vector4D<T> Multiply<T>(
			Quaternion<T> quat,
			Vector4D<T> vec)
			where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
		{
			Transform(
				in vec,
				in quat,
				out Vector4D<T> result);

			return result;
		}
	}
}