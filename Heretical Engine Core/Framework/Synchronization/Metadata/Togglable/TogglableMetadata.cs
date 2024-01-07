namespace HereticalSolutions.Synchronization
{
	public class TogglableMetadata
		: ITogglable
	{
		public TogglableMetadata(
			bool active = true)
		{
			Active = active;
		}

		#region ITogglable

		public bool Active { get; private set; }

		public void Toggle(bool active)
		{
			Active = active;
		}

		#endregion
	}
}