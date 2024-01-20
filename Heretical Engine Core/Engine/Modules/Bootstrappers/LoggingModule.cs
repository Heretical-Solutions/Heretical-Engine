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
		public bool DumpLogsOnTearDown { get; set; }

		private ILogger logger;

		public LoggingModule(
			bool dumpLogsOnTearDown = true)
		{
			DumpLogsOnTearDown = dumpLogsOnTearDown;

			SetUp();
		}

		#region IModule

		public string Name => "Logging module";

		public void Load(IApplicationContext context)
		{
			var contextAsCompositionRoot = context as ICompositionRoot;

			var containerBuilder = contextAsCompositionRoot.ContainerBuilder;

			containerBuilder
				.Register(componentContext =>
				{
					ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

					if (DumpLogsOnTearDown)
					{
						string applicationDataFolder = componentContext.ResolveNamed<string>("Application data folder");

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

					logger = ((ILoggerResolver)loggerBuilder).GetLogger<LoggingModule>();

					/*
					containerBuilder
						.Register(componentContext =>
							loggerBuilder)
						.As<ILoggerResolver>();
					*/

					return (ILoggerResolver)loggerBuilder;
				})
			.As<ILoggerResolver>();

			Initialize();
		}

		public void Unload(IApplicationContext context)
		{
			Cleanup();
		}

		#region ILifetimeable

		public void SetUp()
		{
			if (IsSetUp)
				throw new Exception(
					logger.TryFormat<LoggingModule>(
						"ALREADY SET UP"));

			//Set up

			IsSetUp = true;
		}

		public bool IsSetUp { get; private set; } = false;

		public void Initialize(object[] args = null)
		{
			if (!IsSetUp)
			{
				throw new Exception(
					logger.TryFormat<LoggingModule>(
						"MODULE SHOULD BE SET UP BEFORE BEING INITIALIZED"));
			}

			if (IsInitialized)
			{
				throw new Exception(
					logger.TryFormat<LoggingModule>(
						"ATTEMPT TO INITIALIZE MODULE THAT IS ALREADY INITIALIZED"));
			}

			//Initialization


			IsInitialized = true;

			OnInitialized?.Invoke();
		}

		public bool IsInitialized { get; private set; } = false;

		public Action OnInitialized { get; set; }

		public void Cleanup()
		{
			if (!IsInitialized)
				return;

			//Clean up
			logger = null;

			IsInitialized = false;

			OnCleanedUp?.Invoke();
		}

		public Action OnCleanedUp { get; set; }

		public void TearDown()
		{
			if (!IsSetUp)
				return;

			IsSetUp = false;

			Cleanup();

			//Tear down
			


			OnTornDown?.Invoke();

			OnInitialized = null;

			OnCleanedUp = null;

			OnTornDown = null;
		}

		public Action OnTornDown { get; set; }

		#endregion

		#endregion
	}
}