using HereticalSolutions.Persistence;

namespace HereticalSolutions.Logging
{
	public class ConsoleLoggerWithFileDump
		: IFormatLogger,
		  IDumpable
	{
		private readonly List<string> fullLog;

		private ISerializationArgument serializationArgument;

		private ISerializer serializer;

		public ConsoleLoggerWithFileDump(
			List<string> fullLog)
		{
			serializationArgument = default;

			serializer = default;

			this.fullLog = fullLog;
		}

		//Split into a separate method to break the circular dependency between the logger and the serializer
		public void Initialize(
			ISerializationArgument serializationArgument,
			ISerializer serializer)
		{
			this.serializationArgument = serializationArgument;

			this.serializer = serializer;
		}

		#region IFormatLogger

		public bool Active { get; set; } = true;

		public bool LogTypePrefixEnabled { get; set; } = true;

		public bool RichTextFormattingEnabled { get; set; } = true; //unused for now

		#region Log

		public void Log(
			string value)
		{
			if (!Active)
			{
				return;
			}

			if (LogTypePrefixEnabled)
			{
				value = FormatLogWithMessageType(
					value,
					ELogType.LOG);
			}

			Console.WriteLine(
				value);

			fullLog.Add(value);
		}

		public void Log<TSource>(
			string value)
		{
			if (!Active)
			{
				return;
			}

			value = FormatLogWithSourceType(
				value,
				typeof(TSource));

			if (LogTypePrefixEnabled)
			{
				value = FormatLogWithMessageType(
					value,
					ELogType.LOG);
			}

			Console.WriteLine(value);

			fullLog.Add(value);
		}

		public void Log(
			Type logSource,
			string value)
		{
			if (!Active)
			{
				return;
			}

			value = FormatLogWithSourceType(
				value,
				logSource);

			if (LogTypePrefixEnabled)
			{
				value = FormatLogWithMessageType(
					value,
					ELogType.LOG);
			}

			Console.WriteLine(value);

			fullLog.Add(value);
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
			Log<TSource>(value);
		}

		public void Log(
			Type logSource,
			string value,
			object[] arguments)
		{
			Log(
				logSource,
				value);
		}

		#endregion

		#region Warning

		public void LogWarning(
			string value)
		{
			if (!Active)
			{
				return;
			}

			if (LogTypePrefixEnabled)
			{
				value = FormatLogWithMessageType(
					value,
					ELogType.WARNING);
			}

			Console.WriteLine(
				value);

			fullLog.Add(value);
		}

		public void LogWarning<TSource>(
			string value)
		{
			if (!Active)
			{
				return;
			}

			value = FormatLogWithSourceType(
				value,
				typeof(TSource));

			if (LogTypePrefixEnabled)
			{
				value = FormatLogWithMessageType(
					value,
					ELogType.WARNING);
			}

			Console.WriteLine(value);

			fullLog.Add(value);
		}

		public void LogWarning(
			Type logSource,
			string value)
		{
			if (!Active)
			{
				return;
			}

			value = FormatLogWithSourceType(
				value,
				logSource);

			if (LogTypePrefixEnabled)
			{
				value = FormatLogWithMessageType(
					value,
					ELogType.WARNING);
			}

			Console.WriteLine(value);

			fullLog.Add(value);
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
			LogWarning<TSource>(value);
		}

		public void LogWarning(
			Type logSource,
			string value,
			object[] arguments)
		{
			LogWarning(
				logSource,
				value);
		}

		#endregion

		#region Error

		public void LogError(
			string value)
		{
			if (!Active)
			{
				return;
			}

			if (LogTypePrefixEnabled)
			{
				value = FormatLogWithMessageType(
					value,
					ELogType.ERROR);
			}

			Console.WriteLine(
				value);

			fullLog.Add(value);
		}

		public void LogError<TSource>(
			string value)
		{
			if (!Active)
			{
				return;
			}

			value = FormatLogWithSourceType(
				value,
				typeof(TSource));

			if (LogTypePrefixEnabled)
			{
				value = FormatLogWithMessageType(
					value,
					ELogType.ERROR);
			}

			Console.WriteLine(value);

			fullLog.Add(value);
		}

		public void LogError(
			Type logSource,
			string value)
		{
			if (!Active)
			{
				return;
			}

			value = FormatLogWithSourceType(
				value,
				logSource);

			if (LogTypePrefixEnabled)
			{
				value = FormatLogWithMessageType(
					value,
					ELogType.ERROR);
			}

			Console.WriteLine(value);

			fullLog.Add(value);
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
			LogError<TSource>(value);
		}

		public void LogError(
			Type logSource,
			string value,
			object[] arguments)
		{
			LogError(
				logSource,
				value);
		}

		#endregion

		#region Exception

		public void ThrowException(
			string value)
		{
			throw new Exception(value);
		}

		public void ThrowException<TSource>(
			string value)
		{
			value = FormatLogWithSourceType(
				value,
				typeof(TSource));

			throw new Exception(value);
		}

		public void ThrowException(
			Type logSource,
			string value)
		{
			value = FormatLogWithSourceType(
				value,
				logSource);

			throw new Exception(value);
		}

		#endregion

		#endregion

		#region Dumpable

		public void Dump()
		{
			serializer.Serialize<string[]>(
				serializationArgument,
				fullLog.ToArray());
		}

		#endregion

		private string FormatLogWithMessageType(
			string value,
			ELogType logType)
		{
			return $"[{logType.ToString()}] {value}";
		}

		private string FormatLogWithSourceType(
			string value,
			Type logSource)
		{
			return $"[{logSource.Name}] {value}";
		}

		private string FormatLogWithRichText(
			string value)
		{
			return value; //TODO: https://stackoverflow.com/questions/2743260/is-it-possible-to-write-to-the-console-in-colour-in-net
		}
	}
}