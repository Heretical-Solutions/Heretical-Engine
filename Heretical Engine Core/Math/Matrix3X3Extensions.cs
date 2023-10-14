using Silk.NET.Maths;

namespace HereticalSolutions.HereticalEngine.Math
{
	public static class Matrix3X3Extensions
	{
		//Courtesy of OpenTK https://github.com/opentk/opentk/blob/master/src/OpenTK.Mathematics/Matrix/Matrix3.cs
		public static Matrix3X3<T> Zero<T>()
			where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
		{
			return new Matrix3X3<T>(
				Scalar<T>.Zero, Scalar<T>.Zero, Scalar<T>.Zero,
				Scalar<T>.Zero, Scalar<T>.Zero, Scalar<T>.Zero,
				Scalar<T>.Zero, Scalar<T>.Zero, Scalar<T>.Zero);
		}
	}
}