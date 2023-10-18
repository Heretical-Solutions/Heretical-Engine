using Silk.NET.Maths;

using System.Numerics;

namespace HereticalSolutions.HereticalEngine.Math
{
	public static class Vector2DExtensions
	{
		public static Vector2D<float> ToSilkNetVector2D(this Vector2 vector)
		{
			return new Vector2D<float>(vector.X, vector.Y);
		}

		public static Vector2 ToNumericsVector2(this Vector2D<float> vector)
		{
			return new Vector2(vector.X, vector.Y);
		}
	}
}