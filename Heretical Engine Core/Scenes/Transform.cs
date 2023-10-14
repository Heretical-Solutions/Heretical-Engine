using Silk.NET.Maths;

using HereticalSolutions.HereticalEngine.Math;

namespace HereticalSolutions.HereticalEngine.Scenes
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

		public Matrix4X4<float> TRSMatrix;

		public bool Dirty;

		#endregion

		public void RecalculateTRSMatrix()
		{
			TRSMatrix = RotationMatrix * ScaleMatrix * TranslationMatrix;

			Dirty = false;
		}

		/*
		public Vector4D<float> CalculateScreenPoint(
			Vector4D<float> localPosition)
		{
			return localPosition * MVPMatrix;
		}
		*/

		#region Matrix calculations

		/*
		public Matrix4X4<float> MVPMatrix { get => ProjectionMatrix * ViewMatrix * ModelMatrix; }

		public Matrix4X4<float> ProjectionMatrix { get { return default; } }

		public Matrix4X4<float> ViewMatrix { get => -TranslationMatrix * ScaleMatrix * RotationMatrix; }

		//TRS matrix
		public Matrix4X4<float> ModelMatrix { get =>  RotationMatrix * ScaleMatrix * TranslationMatrix; }
		*/

		public Matrix4X4<float> TranslationMatrix { get => Matrix4X4.CreateTranslation(Position); }

		public Matrix4X4<float> RotationMatrix { get => Matrix4X4.CreateFromQuaternion(Rotation); }

		public Matrix4X4<float> ScaleMatrix { get => Matrix4X4.CreateScale(Scale); }

		#endregion

		#region Rotations

		public void Rotate(
			Quaternion<float> rot,
			ESceneSpace space = ESceneSpace.LOCAL)
		{
			if (space == ESceneSpace.LOCAL)
				Rotation = Rotation * rot;
			else
				Rotation = rot * Rotation;

			RecalculateTRSMatrix();
		}

		public void Rotate(
			Vector3D<float> angles,
			ESceneSpace space = ESceneSpace.LOCAL)
		{
			//Rotate(Quaternion<float>.FromEulerAngles(angles), space);
			Rotate(
				QuaternionExtensions.CreateQuaternionFromEulerAngles(
					angles),
				space);
		}

		public void Rotate(
			Vector3D<float> direction,
			float angle, ESceneSpace space = ESceneSpace.LOCAL)
		{
			Rotate(
				Quaternion<float>.CreateFromAxisAngle(
					direction,
					angle),
				space);
		}

		public void LookAt(
			Vector3D<float> position,
			Vector3D<float> up)
		{
			//Rotation = Matrix4X4.CreateLookAt(this.position, position, up).ExtractRotation();
			Rotation = Quaternion<float>.CreateFromRotationMatrix(
				Matrix4X4.CreateLookAt(
					Position,
					position,
					up));

			RecalculateTRSMatrix();
		}

		public void LookAt(Vector3D<float> position)
		{
			LookAt(
				position,
				Up);
		}

		#endregion
	}
}