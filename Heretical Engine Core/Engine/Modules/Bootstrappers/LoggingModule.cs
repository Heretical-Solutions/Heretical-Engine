using System.Globalization;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Logging.Factories;

using HereticalSolutions.Logging;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class LoggingModule
		: IModule
	{
		private readonly bool dumpLogsOnTearDown;

		public LoggingModule(
			bool dumpLogsOnTearDown = true)
		{
			this.dumpLogsOnTearDown = dumpLogsOnTearDown;
		}

		#region IModule

		public string Name => "Logging module";

		public void Load(IApplicationContext context)
		{
			var compositionRoot = context as ICompositionRoot;

			var containerBuilder = compositionRoot.ContainerBuilder;

			containerBuilder
				.Register(componentContext =>
				{
					ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

					if (dumpLogsOnTearDown)
					{
						if (!componentContext.TryResolveNamed<string>(
							ApplicationDataConstants.APPLICATION_DATA_FOLDER,
							out string applicationDataFolder))
						{
							throw new Exception("[LoggingModule] COULD NOT RESOLVE APPLICATION DATA FOLDER");
						}

						//Courtesy of https://stackoverflow.com/questions/114983/given-a-datetime-object-how-do-i-get-an-iso-8601-date-in-string-format
						//Read comments carefully

						// Prefer this, to avoid having to manually define a framework-provided format
						//string dateTimeNow = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

						string dateTimeNow = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture);

						//string dateTimeNow = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

						string logFileName = dateTimeNow;


						loggerBuilder
							.ToggleAllowedByDefault(true)
							.AddOrWrap(
								LoggersFactory.BuildConsoleLogger())
							.AddOrWrap(
								LoggersFactory.BuildLoggerWrapperWithFileDump(
									applicationDataFolder,
									$"Runtime logs/{logFileName}.log",
									(ILoggerResolver)loggerBuilder,
									loggerBuilder.CurrentLogger))
							.AddOrWrap(
								LoggersFactory.BuildLoggerWrapperWithLogTypePrefix(
									loggerBuilder.CurrentLogger))
							.AddOrWrap(
								LoggersFactory.BuildLoggerWrapperWithSourceTypePrefix(
									loggerBuilder.CurrentLogger));
					}
					else
					{
						loggerBuilder
							.ToggleAllowedByDefault(true)
							.AddOrWrap(
								LoggersFactory.BuildConsoleLogger())
							.AddOrWrap(
								LoggersFactory.BuildLoggerWrapperWithLogTypePrefix(
									loggerBuilder.CurrentLogger))
							.AddOrWrap(
								LoggersFactory.BuildLoggerWrapperWithSourceTypePrefix(
									loggerBuilder.CurrentLogger));
					}

					ILoggerResolver loggerResolver = (ILoggerResolver)loggerBuilder;

					var logger = loggerResolver.GetLogger<LoggingModule>();

					logger?.Log<LoggingModule>(
						"LOGGER RESOLVER BUILT");

					return loggerResolver;
				})
				.As<ILoggerResolver>()
				.SingleInstance();

			containerBuilder
				.Register(componentContext =>
				{
					if (!componentContext.TryResolve<ILoggerResolver>(
						out ILoggerResolver loggerResolver))
						return null;

					ILoggerBuilder loggerBuilder = (ILoggerBuilder)loggerResolver;

					var currentLogger = loggerBuilder.CurrentLogger;

					do
					{
						if (currentLogger is IDumpable dumpableLogger)
						{
							return (IDumpable)currentLogger;
						}

						if (currentLogger is not ILoggerWrapper loggerWrapper)
						{
							return null;
						}

						currentLogger = loggerWrapper.InnerLogger;
					}
					while (true);
				})
				.As<IDumpable>()
				.SingleInstance();
		}

		public void Unload(IApplicationContext context)
		{
			if (((ICompositionRoot)context)
				.DIContainer
				.TryResolve<IDumpable>(
					out IDumpable dumpableLogger))
			{
				dumpableLogger.Dump();
			}
		}

		#endregion
	}
}