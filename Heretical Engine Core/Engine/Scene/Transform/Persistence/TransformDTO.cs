using System.Numerics;

namespace HereticalSolutions.HereticalEngine.Scenes
{
	[Serializable]
	public struct TransformDTO
	{
		public Vector3 Position;

		public Quaternion Rotation;

		public Vector3 Scale;
	}
}