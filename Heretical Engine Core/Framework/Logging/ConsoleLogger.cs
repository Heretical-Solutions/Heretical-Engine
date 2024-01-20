namespace HereticalSolutions.Logging
{
	public class ConsoleLogger
		: ILogger
	{
		#region ILogger

		#region Log

		public void Log(
			string value)
		{
			Console.WriteLine(
				value);
		}

		public void Log<TSource>(
			string value)
		{
			Log(value);
		}

		public void Log(
			Type logSource,
			string value)
		{
			Log(value);
		}

		public void Log(
			string value,
			object[] arguments)
		{
			Log(value);
		}

		public void Log<TSource>(
			string value,
			object[] arguments)
		{
			Log(value);
		}

		public void Log(
			Type logSource,
			string value,
			object[] arguments)
		{
			Log(value);
		}

		#endregion

		#region Warning

		public void LogWarning(
			string value)
		{
			Console.WriteLine(
				value);
		}

		public void LogWarning<TSource>(
			string value)
		{
			LogWarning(value);
		}

		public void LogWarning(
			Type logSource,
			string value)
		{
			LogWarning(value);
		}

		public void LogWarning(
			string value,
			object[] arguments)
		{
			LogWarning(value);
		}

		public void LogWarning<TSource>(
			string value,
			object[] arguments)
		{
			LogWarning(value);
		}

		public void LogWarning(
			Type logSource,
			string value,
			object[] arguments)
		{
			LogWarning(value);
		}

		#endregion

		#region Error

		public void LogError(
			string value)
		{
			Console.WriteLine(
				value);
		}

		public void LogError<TSource>(
			string value)
		{
			LogError(value);
		}

		public void LogError(
			Type logSource,
			string value)
		{
			LogError(value);
		}

		public void LogError(
			string value,
			object[] arguments)
		{
			LogError(value);
		}

		public void LogError<TSource>(
			string value,
			object[] arguments)
		{
			LogError(value);
		}

		public void LogError(
			Type logSource,
			string value,
			object[] arguments)
		{
			LogError(value);
		}

		#endregion

		#region Exception

		public string FormatException(
			string value)
		{
			return value;
		}

		public string FormatException<TSource>(
			string value)
		{
			return value;
		}

		public string FormatException(
			Type logSource,
			string value)
		{
			return value;
		}

		#endregion

		#endregion

		/*
		private string FormatLogWithRichText(
			string value)
		{
			return value; //TODO: https://stackoverflow.com/questions/2743260/is-it-possible-to-write-to-the-console-in-colour-in-net
		}
		*/
	}
}