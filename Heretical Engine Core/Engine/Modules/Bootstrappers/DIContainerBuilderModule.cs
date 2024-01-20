using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class DIContainerBuilderModule
		: IModule
	{
		public DIContainerBuilderModule()
		{
			SetUp();
		}

		#region IModule

		public string Name => "DI container builder module";

		public void Load(IApplicationContext context)
		{
			var compositionRoot = context as ICompositionRoot;

			compositionRoot.BuildContainer();

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
				return;

			//Set up

			IsSetUp = true;
		}

		public bool IsSetUp { get; private set; } = false;

		public void Initialize(object[] args = null)
		{
			if (!IsSetUp)
			{
				return;
			}

			if (IsInitialized)
			{
				return;
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