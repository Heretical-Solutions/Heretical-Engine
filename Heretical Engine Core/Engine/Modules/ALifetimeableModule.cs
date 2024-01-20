using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public abstract class ALifetimeableModule
		: ALifetimeable,
		  IModule
	{
		public ALifetimeableModule(
			ILogger logger = null)
			: base(logger)
		{
			SetUp();
		}

		#region IModule

		public abstract string Name { get; }

		public virtual void Load(IApplicationContext context)
		{
			Initialize();
		}

		public virtual void Unload(IApplicationContext context)
		{
			Cleanup();
		}

		#endregion
	}
}