using HereticalSolutions.Persistence;

namespace HereticalSolutions.Logging
{
	public class LoggerWrapperWithFileDump
		: ILogger,
		  ILoggerWrapper,
		  IDumpable
	{
		private readonly ILogger innerLogger;

		private readonly ISerializationArgument serializationArgument;

		private readonly ISerializer serializer;

		private readonly List<string> fullLog;

		public LoggerWrapperWithFileDump(
			ILogger innerLogger,
			ISerializationArgument serializationArgument,
			ISerializer serializer,
			List<string> fullLog)
		{
			this.innerLogger = innerLogger;

			this.serializationArgument = serializationArgument;

			this.serializer = serializer;

			this.fullLog = fullLog;
		}

		#region ILoggerWrapper

		public ILogger InnerLogger { get => innerLogger; }

		#endregion

		#region ILogger

		#region Log

		public void Log(
			string value)
		{
			fullLog.Add(value);

			innerLogger.Log(value);
		}

		public void Log<TSource>(
			string value)
		{
			fullLog.Add(value);

			innerLogger.Log<TSource>(value);
		}

		public void Log(
			Type logSource,
			string value)
		{
			fullLog.Add(value);

			innerLogger.Log(
				logSource,
				value);
		}

		public void Log(
			string value,
			object[] arguments)
		{
			fullLog.Add(value);

			innerLogger.Log(
				value,
				arguments);
		}

		public void Log<TSource>(
			string value,
			object[] arguments)
		{
			fullLog.Add(value);

			innerLogger.Log<TSource>(
				value,
				arguments);
		}

		public void Log(
			Type logSource,
			string value,
			object[] arguments)
		{
			fullLog.Add(value);

			innerLogger.Log(
				logSource,
				value,
				arguments);
		}

		#endregion

		#region Warning

		public void LogWarning(
			string value)
		{
			fullLog.Add(value);

			innerLogger.LogWarning(
				value);
		}

		public void LogWarning<TSource>(
			string value)
		{
			fullLog.Add(value);

			innerLogger.LogWarning<TSource>(value);
		}

		public void LogWarning(
			Type logSource,
			string value)
		{
			fullLog.Add(value);

			innerLogger.LogWarning(
				logSource,
				value);
		}

		public void LogWarning(
			string value,
			object[] arguments)
		{
			fullLog.Add(value);

			innerLogger.LogWarning(
				value,
				arguments);
		}

		public void LogWarning<TSource>(
			string value,
			object[] arguments)
		{
			fullLog.Add(value);

			innerLogger.LogWarning<TSource>(
				value,
				arguments);
		}

		public void LogWarning(
			Type logSource,
			string value,
			object[] arguments)
		{
			fullLog.Add(value);

			innerLogger.LogWarning(
				logSource,
				value,
				arguments);
		}

		#endregion

		#region Error

		public void LogError(
			string value)
		{
			fullLog.Add(value);

			innerLogger.LogError(
				value);
		}

		public void LogError<TSource>(
			string value)
		{
			fullLog.Add(value);

			innerLogger.LogError<TSource>(value);
		}

		public void LogError(
			Type logSource,
			string value)
		{
			fullLog.Add(value);

			innerLogger.LogError(
				logSource,
				value);
		}

		public void LogError(
			string value,
			object[] arguments)
		{
			fullLog.Add(value);

			innerLogger.LogError(
				value,
				arguments);
		}

		public void LogError<TSource>(
			string value,
			object[] arguments)
		{
			fullLog.Add(value);

			innerLogger.LogError<TSource>(
				value,
				arguments);
		}

		public void LogError(
			Type logSource,
			string value,
			object[] arguments)
		{
			fullLog.Add(value);

			innerLogger.LogError(
				logSource,
				value,
				arguments);
		}

		#endregion

		#region Exception

		public string FormatException(
			string value)
		{
			var result = innerLogger.FormatException(value);

			fullLog.Add(result);

			return result;	
		}

		public string FormatException<TSource>(
			string value)
		{
			var result = innerLogger.FormatException<TSource>(value);

			fullLog.Add(result);

			return result;
		}

		public string FormatException(
			Type logSource,
			string value)
		{
			var result = innerLogger.FormatException(				
				logSource,
				value);

			fullLog.Add(result);

			return result;
		}

		#endregion

		#endregion

		#region Dumpable

		public void Dump()
		{
			Log(
				GetType(),
				$"DUMPING LOGS TO FILE");

			serializer.Serialize<string[]>(
				serializationArgument,
				fullLog.ToArray());
		}

		#endregion
	}
}