namespace HereticalSolutions
{
	public static class PathHelpers
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