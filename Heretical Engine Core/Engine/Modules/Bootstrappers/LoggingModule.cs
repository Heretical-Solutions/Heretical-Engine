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
		private const string KEY_DUMPABLE_LOGGER = "Dumpable logger";

		public bool DumpLogsOnTearDown { get; set; }

		public LoggingModule(
			bool dumpLogsOnTearDown = true)
		{
			DumpLogsOnTearDown = dumpLogsOnTearDown;
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

					if (DumpLogsOnTearDown)
					{
						if (!componentContext.TryResolveNamed<string>(
							ApplicationDataConstants.APPLICATION_DATA_FOLDER,
							out string applicationDataFolder))
							throw new Exception("[LoggingModule] COULD NOT RESOLVE APPLICATION DATA FOLDER");

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
									loggerBuilder.CurrentLogger));

						var dumpableLogger = loggerBuilder.CurrentLogger;

						containerBuilder
							.Register(componentContext =>
								(IDumpable)dumpableLogger)
							.Named<IDumpable>(KEY_DUMPABLE_LOGGER);

						loggerBuilder
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

					return (ILoggerResolver)loggerBuilder;
				})
			.As<ILoggerResolver>();
		}

		public void Unload(IApplicationContext context)
		{
			if (DumpLogsOnTearDown)
			{
				if (context
					.DIContainer
					.TryResolveNamed<IDumpable>(
						KEY_DUMPABLE_LOGGER,
						out var dumpableLogger))
				{
					dumpableLogger.Dump();
				}
			}
		}

		#endregion
	}
}