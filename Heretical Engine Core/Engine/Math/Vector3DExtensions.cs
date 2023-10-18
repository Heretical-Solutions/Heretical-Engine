using Silk.NET.Maths;

using System.Numerics;

namespace HereticalSolutions.HereticalEngine.Math
{
	public static class Vector3DExtensions
	{
		public static Vector3D<float> ToSilkNetVector3D(this Vector3 vector)
		{
			return new Vector3D<float>(vector.X, vector.Y, vector.Z);
		}

		public static Vector3 ToNumericsVector3(this Vector3D<float> vector)
		{
			return new Vector3(vector.X, vector.Y, vector.Z);
		}
	}
}