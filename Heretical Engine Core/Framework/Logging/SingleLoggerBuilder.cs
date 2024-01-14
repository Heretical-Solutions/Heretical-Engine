using HereticalSolutions.Repositories;

namespace HereticalSolutions.Logging
{
	public class SingleLoggerBuilder
		: ILoggerBuilder,
		  ILoggerResolver
	{
		private readonly IFormatLogger logger;

		private readonly IRepository<Type, bool> explicitLogSourceRules;

		public SingleLoggerBuilder(
			IFormatLogger logger,
			IRepository<Type, bool> explicitLogSourceRules,
			bool allowedByDefault = true)
		{
			this.logger = logger;

			this.explicitLogSourceRules = explicitLogSourceRules;

			AllowedByDefault = allowedByDefault;
		}

		#region ILoggerBuilder

		public bool AllowedByDefault { get; set; }

		public void Toggle<TLogSource>(
			bool allowed)
		{
			explicitLogSourceRules.AddOrUpdate(
				typeof(TLogSource),
				allowed);
		}

		public void Toggle(
			Type logSourceType,
			bool allowed)
		{
			explicitLogSourceRules.AddOrUpdate(
				logSourceType,
				allowed);
		}

		#endregion

		#region ILoggerResolver

		public IFormatLogger GetLogger<TLogSource>()
		{
			return GetLogger(
				typeof(TLogSource));
		}

		public IFormatLogger GetLogger(Type logSourceType)
		{
			bool provide = AllowedByDefault;

			if (explicitLogSourceRules.Has(
				logSourceType))
			{
				provide = explicitLogSourceRules.Get(logSourceType);
			}

			if (provide)
			{
				return logger;
			}

			return null;
		}

		#endregion
	}
}