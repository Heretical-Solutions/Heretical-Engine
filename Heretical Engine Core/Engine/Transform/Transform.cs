using Silk.NET.Maths;

using Newtonsoft.Json;

namespace HereticalSolutions.HereticalEngine
{
	public struct Transform
	{
		#region Constants

		public static Vector3D<float> Up = Vector3D<float>.UnitY;
		public static Vector3D<float> Down = -Vector3D<float>.UnitY;

		public static Vector3D<float> Right = Vector3D<float>.UnitX;
		public static Vector3D<float> Left = -Vector3D<float>.UnitX;

		public static Vector3D<float> Backward = Vector3D<float>.UnitZ;
		public static Vector3D<float> Forward = -Vector3D<float>.UnitZ;

		#endregion

		#region Variables

		public Vector3D<float> Position;

		public Quaternion<float> Rotation;

		public Vector3D<float> Scale;

		#endregion

		#region Matrix calculations

		[JsonIgnore] //TODO: https://www.newtonsoft.com/json/help/html/conditionalproperties.htm
		public Matrix4X4<float> TranslationMatrix { get => Matrix4X4.CreateTranslation(Position); }

		[JsonIgnore] //TODO: https://www.newtonsoft.com/json/help/html/conditionalproperties.htm
		public Matrix4X4<float> RotationMatrix { get => Matrix4X4.CreateFromQuaternion(Rotation); }

		[JsonIgnore] //TODO: https://www.newtonsoft.com/json/help/html/conditionalproperties.htm
		public Matrix4X4<float> ScaleMatrix { get => Matrix4X4.CreateScale(Scale); }

		//Model space TRS matrix
		[JsonIgnore] //TODO: https://www.newtonsoft.com/json/help/html/conditionalproperties.htm
		public Matrix4X4<float> TRSMatrix { get => RotationMatrix * ScaleMatrix * TranslationMatrix; }

		public void DecomposeTRSMatrix(Matrix4X4<float> TRSMatrix)
		{
			Matrix4X4.Decompose<float>(
				TRSMatrix,
				out Scale,
				out Rotation,
				out Position);
		}

		#endregion
	}
}