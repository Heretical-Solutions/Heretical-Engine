using Silk.NET.Maths;

using Newtonsoft.Json;

namespace HereticalSolutions.HereticalEngine
{
	public struct WorldTransformComponent
	{
		public Transform Transform;

		[JsonIgnore] //TODO: https://www.newtonsoft.com/json/help/html/conditionalproperties.htm
		public Matrix4X4<float> TRSMatrix;

		[JsonIgnore] //TODO: https://www.newtonsoft.com/json/help/html/conditionalproperties.htm
		public bool Dirty;
	}
}