using HereticalSolutions.Persistence;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	[Serializable]
	public class ShaderSourceDTO
		: IContainsPlainText
	{
		public string Text { get; set;}
	}
}