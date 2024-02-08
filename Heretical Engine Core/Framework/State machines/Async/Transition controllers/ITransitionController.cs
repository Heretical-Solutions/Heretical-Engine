using System;
using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.StateMachines
{
    /// <summary>
    /// Represents the interface for a transition controller
    /// </summary>
    /// <typeparam name="TBaseState">The base state type.</typeparam>
    public interface ITransitionController<TBaseState>
        where TBaseState : IState
    {
        /// <summary>
        /// Enters the specified state
        /// </summary>
        /// <param name="state">The state to enter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="progress">The progress reporting object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task EnterState(
            TBaseState state,
            CancellationToken cancellationToken,
            IProgress<float> progress = null);

        /// <summary>
        /// Exits the specified state
        /// </summary>
        /// <param name="state">The state to exit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="progress">The progress reporting object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ExitState(
            TBaseState state,
            CancellationToken cancellationToken,
            IProgress<float> progress = null);
    }
}