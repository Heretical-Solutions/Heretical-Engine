namespace HereticalSolutions
{
	public static class PathExtensions
	{
		public static string SanitizePath(this string path)
		{
			return path
				.Replace(
					"\\",
					"/")
				.Replace(
					"\\\\",
					"/");
		}
	}
}