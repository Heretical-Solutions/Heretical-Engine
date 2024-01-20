namespace HereticalSolutions.Logging
{
	public static class LoggingExtensions
	{
		public static string TryFormat(
			this ILogger logger,
			string value)
		{
			if (logger != null)
				value = logger.FormatException(value);

			return value;
		}

		public static string TryFormat<TSource>(
			this ILogger logger,
			string value)
		{
			if (logger != null)
				value = logger.FormatException<TSource>(value);

			return value;
		}

		public static string TryFormat(
			this ILogger logger,
			Type logSource,
			string value)
		{
			if (logger != null)
				value = logger.FormatException(
					logSource,
					value);

			return value;
		}
	}
}