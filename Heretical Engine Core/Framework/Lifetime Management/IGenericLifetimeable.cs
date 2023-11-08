using System;

namespace HereticalSolutions.LifetimeManagement
{
	/// <summary>
	/// Represents an object that can be managed throughout its lifetime.
	/// </summary>
	public interface IGenericLifetimeable<TContext>
	{
		#region Set up

		void SetUp(
			TContext context);

		bool IsSetUp { get; }

		#endregion

		#region Initialize

		void Initialize(
			TContext context);

		bool IsInitialized { get; }

		Action OnInitialized { get; set; }

		#endregion

		#region Cleanup

		void Cleanup();

		Action OnCleanedUp { get; set; }

		#endregion

		#region Tear down

		void TearDown();

		Action OnTornDown { get; set; }

		#endregion
	}
}