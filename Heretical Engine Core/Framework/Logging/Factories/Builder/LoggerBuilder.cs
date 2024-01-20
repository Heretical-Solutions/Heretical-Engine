using HereticalSolutions.Repositories;

namespace HereticalSolutions.Logging
{
	public class LoggerBuilder
		: ILoggerBuilder,
		  ILoggerResolver
	{
		private readonly IRepository<Type, bool> explicitLogSourceRules;

		private ILogger currentLogger;

		private bool allowedByDefault;

		public LoggerBuilder(
			IRepository<Type, bool> explicitLogSourceRules)
		{
			this.explicitLogSourceRules = explicitLogSourceRules;

			currentLogger = null;

			allowedByDefault = true;
		}

		#region ILoggerBuilder

		public ILogger CurrentLogger { get => currentLogger; }

		public bool CurrentAllowedByDefault { get => allowedByDefault; }

		public ILoggerBuilder ToggleAllowedByDefault(
			bool allowed)
		{
			allowedByDefault = allowed;

			return this;
		}

		public ILoggerBuilder ToggleLogSource<TLogSource>(
			bool allowed)
		{
			explicitLogSourceRules.AddOrUpdate(
				typeof(TLogSource),
				allowed);

			return this;
		}

		public ILoggerBuilder ToggleLogSource(
			Type logSourceType,
			bool allowed)
		{
			explicitLogSourceRules.AddOrUpdate(
				logSourceType,
				allowed);

			return this;
		}

		public ILoggerBuilder AddOrWrap(ILogger logger)
		{
			currentLogger = logger;

			return this;
		}

		#endregion

		#region ILoggerResolver

		public ILogger GetLogger<TLogSource>()
		{
			return GetLogger(
				typeof(TLogSource));
		}

		public ILogger GetLogger(Type logSourceType)
		{
			bool provide = allowedByDefault;

			if (explicitLogSourceRules.Has(
				logSourceType))
			{
				provide = explicitLogSourceRules.Get(logSourceType);
			}

			if (provide)
			{
				return currentLogger;
			}

			return null;
		}

		#endregion
	}
}