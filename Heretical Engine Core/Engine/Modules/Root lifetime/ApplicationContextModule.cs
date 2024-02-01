using HereticalSolutions.HereticalEngine.Application;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class ApplicationContextModule
		: ALifetimeableModule
	{
		public override string Name => "Application context module";

		protected override void InitializeInternal()
		{
			var compositionRoot = context as ICompositionRoot;

			var containerBuilder = compositionRoot.ContainerBuilder;

			containerBuilder
				.Register(componentContext =>
				{
					//TODO: maybe come up with a better way
					return (ApplicationContext)context;
				})
				.As<ICompositionRoot>()
				.As<IApplicationContext>()
				.As<IRootLifetimeManager>()
				.As<IApplicationStatusManager>()
				.As<IModuleManager>()
				.As<IActiveModuleRegistry>()
				.SingleInstance();

			base.InitializeInternal();
		}
	}
}