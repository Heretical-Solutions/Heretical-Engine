namespace HereticalSolutions.HereticalEngine.Messaging
{
	public class MainThreadCommand
	{
		public ECommandStatus Status
		{
			get
			{
				ECommandStatus result;

				lock (lockObject)
				{
					result = status;
				}

				return result;
			}
		}

		public Action DelegateToPerform { get; private set; }

		public Func<Task> AsyncDelegateToPerform { get; private set; }

		public bool Async { get; private set; }


		private ECommandStatus status;

		private object lockObject = new object();


		public MainThreadCommand(
			Action delegateToPerform)
		{
			status = ECommandStatus.QUEUED;

			DelegateToPerform = delegateToPerform;

			AsyncDelegateToPerform = null;

			Async = false;
		}

		public MainThreadCommand(
			Func<Task> asyncDelegateToPerform)
		{
			status = ECommandStatus.QUEUED;

			AsyncDelegateToPerform = asyncDelegateToPerform;

			DelegateToPerform = null;

			Async = true;
		}

		public void Execute()
		{
			lock (lockObject)
			{
				status = ECommandStatus.IN_PROGRESS;
			}

			DelegateToPerform?.Invoke();

			lock (lockObject)
			{
				status = ECommandStatus.DONE;
			}
		}

		public async Task ExecuteAsync()
		{
			lock (lockObject)
			{
				status = ECommandStatus.IN_PROGRESS;
			}

			//TODO: .ThrowExceptions() here
			await AsyncDelegateToPerform?
				.Invoke();

			lock (lockObject)
			{
				status = ECommandStatus.DONE;
			}
		}
	}
}