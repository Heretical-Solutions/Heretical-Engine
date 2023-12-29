using Silk.NET.Maths;

using Newtonsoft.Json;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	//It's called Camera Pose now
	//Decided that calling it simply a "Camera" may ambiguate it once I start digging deeper into this rendering rabbit's hole
	//Courtesy of https://en.wikipedia.org/wiki/Pose_(computer_vision)
	public struct CameraPose
	{
		//For the comments below, courtesy of http://www.codinglabs.net/article_world_view_projection_matrix.aspx

		[JsonIgnore] //TODO: https://www.newtonsoft.com/json/help/html/conditionalproperties.htm
		public Matrix4X4<float> ViewMatrix; //Converts World space into View space

		[JsonIgnore] //TODO: https://www.newtonsoft.com/json/help/html/conditionalproperties.htm
		public Matrix4X4<float> ProjectionMatrix; //Converts View space into Projection space
	}
}