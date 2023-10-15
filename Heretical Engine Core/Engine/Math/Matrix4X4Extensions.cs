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
	}
}