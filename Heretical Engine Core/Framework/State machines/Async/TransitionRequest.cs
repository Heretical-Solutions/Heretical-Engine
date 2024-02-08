using System;
using System.Threading;

namespace HereticalSolutions.StateMachines
{
    /// <summary>
    /// Represents a transition request for a state machine
    /// </summary>
    /// <typeparam name="TBaseState">The base state type.</typeparam>
    public class TransitionRequest<TBaseState>
        where TBaseState : IState
    {
        /// <summary>
        /// Gets the transition event associated with the request
        /// </summary>
        public ITransitionEvent<TBaseState> Event { get; private set; }

        /// <summary>
        /// Gets the cancellation token source for the request
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; private set; }
        
        /// <summary>
        /// Gets the progress reporting object for the state exit progress
        /// </summary>
        public IProgress<float> StateExitProgress { get; private set; }
        
        /// <summary>
        /// Gets the progress reporting object for the state enter progress
        /// </summary>
        public IProgress<float> StateEnterProgress { get; private set; }

        /// <summary>
        /// Gets or sets the transition protocol for the request
        /// </summary>
        public TransitionProtocol TransitionProtocol { get; private set; }

        /// <summary>
        /// Gets or sets the state of the transition
        /// </summary>
        public EAsyncTransitionState TransitionState { get; set; }

        /// <summary>
        /// Initializes a new instance of the TransitionRequest class with the specified parameters
        /// </summary>
        /// <param name="event">The transition event.</param>
        /// <param name="cancellationTokenSource">The cancellation token source.</param>
        /// <param name="stateExitProgress">The progress reporting object for the state exit progress.</param>
        /// <param name="stateEnterProgress">The progress reporting object for the state enter progress.</param>
        /// <param name="transitionProtocol">The transition protocol.</param>
        public TransitionRequest(
            ITransitionEvent<TBaseState> @event,
            CancellationTokenSource cancellationTokenSource,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol transitionProtocol = null)
        {
            Event = @event;

            CancellationTokenSource = cancellationTokenSource;

            StateExitProgress = stateExitProgress;

            StateEnterProgress = stateEnterProgress;

            TransitionProtocol = transitionProtocol;
            
            TransitionState = EAsyncTransitionState.QUEUED;
        }
    }
}