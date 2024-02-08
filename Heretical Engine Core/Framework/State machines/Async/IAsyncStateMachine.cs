using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.StateMachines
{
    public interface IAsyncStateMachine<TBaseState>
        where TBaseState : IState
    {
        /// <summary>
        /// Gets a value indicating whether a transition is in progress
        /// </summary>
        bool TransitionInProgress { get; }
        
        #region Current state

        /// <summary>
        /// Gets the current state of the state machine
        /// </summary>
        TBaseState CurrentState { get; }
        
        /// <summary>
        /// Gets or sets the action to be executed when the current state change starts
        /// </summary>
        Action<TBaseState, TBaseState> OnCurrentStateChangeStarted { get; set; }
        
        /// <summary>
        /// Gets or sets the action to be executed when the current state change finishes
        /// </summary>
        Action<TBaseState, TBaseState> OnCurrentStateChangeFinished { get; set; }

        #endregion

        #region All states

        /// <summary>
        /// Gets the state of the specified concrete state type
        /// </summary>
        /// <typeparam name="TConcreteState">The type of the concrete state.</typeparam>
        /// <returns>The state of the specified concrete state type.</returns>
        TBaseState GetState<TConcreteState>();
        
        /// <summary>
        /// Gets the state of the specified state type
        /// </summary>
        /// <param name="stateType">The type of the state.</param>
        /// <returns>The state of the specified state type.</returns>
        TBaseState GetState(Type stateType);
        
        /// <summary>
        /// Gets all the state types present in the state machine
        /// </summary>
        IEnumerable<Type> AllStates { get; }

        #endregion
        
        #region Event handling
        
        /// <summary>
        /// Handles the specified event with optional progress reporting and transition protocol
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to handle.</typeparam>
        /// <param name="stateExitProgress">The progress of the state exit.</param>
        /// <param name="stateEnterProgress">The progress of the state enter.</param>
        /// <param name="protocol">The transition protocol to use.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Handle<TEvent>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null);
        
        /// <summary>
        /// Handles the specified event with optional progress reporting and transition protocol
        /// </summary>
        /// <param name="eventType">The type of the event to handle.</param>
        /// <param name="stateExitProgress">The progress of the state exit.</param>
        /// <param name="stateEnterProgress">The progress of the state enter.</param>
        /// <param name="protocol">The transition protocol to use.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Handle(
            Type eventType,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null);
        
        /// <summary>
        /// Handles the specified event with optional progress reporting, transition protocol, and cancellation token
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to handle.</typeparam>
        /// <param name="token">The cancellation token.</param>
        /// <param name="stateExitProgress">The progress of the state exit.</param>
        /// <param name="stateEnterProgress">The progress of the state enter.</param>
        /// <param name="protocol">The transition protocol to use.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Handle<TEvent>(
            CancellationToken token,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null);
        
        /// <summary>
        /// Handles the specified event with optional progress reporting, transition protocol, and cancellation token
        /// </summary>
        /// <param name="eventType">The type of the event to handle.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="stateExitProgress">The progress of the state exit.</param>
        /// <param name="stateEnterProgress">The progress of the state enter.</param>
        /// <param name="protocol">The transition protocol to use.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Handle(
            Type eventType,
            CancellationToken token,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null);
        
        /// <summary>
        /// Gets or sets the action to be executed when an event is fired
        /// </summary>
        Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }
        
        #endregion

        #region Immediate transition
        
        /// <summary>
        /// Transitions immediately to the specified state with optional progress reporting and transition protocol
        /// </summary>
        /// <typeparam name="TState">The type of the state to transition to.</typeparam>
        /// <param name="stateExitProgress">The progress of the state exit.</param>
        /// <param name="stateEnterProgress">The progress of the state enter.</param>
        /// <param name="protocol">The transition protocol to use.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task TransitToImmediately<TState>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null);
        
        /// <summary>
        /// Transitions immediately to the specified state with optional progress reporting and transition protocol
        /// </summary>
        /// <param name="stateType">The type of the state to transition to.</param>
        /// <param name="stateExitProgress">The progress of the state exit.</param>
        /// <param name="stateEnterProgress">The progress of the state enter.</param>
        /// <param name="protocol">The transition protocol to use.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task TransitToImmediately(
            Type stateType,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null);
        
        /// <summary>
        /// Transitions immediately to the specified state with optional progress reporting, transition protocol, and cancellation token
        /// </summary>
        /// <typeparam name="TState">The type of the state to transition to.</typeparam>
        /// <param name="token">The cancellation token.</param>
        /// <param name="stateExitProgress">The progress of the state exit.</param>
        /// <param name="stateEnterProgress">The progress of the state enter.</param>
        /// <param name="protocol">The transition protocol to use.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task TransitToImmediately<TState>(
            CancellationToken token,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null);
        
        /// <summary>
        /// Transitions immediately to the specified state with optional progress reporting, transition protocol, and cancellation token
        /// </summary>
        /// <param name="stateType">The type of the state to transition to.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="stateExitProgress">The progress of the state exit.</param>
        /// <param name="stateEnterProgress">The progress of the state enter.</param>
        /// <param name="protocol">The transition protocol to use.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task TransitToImmediately(
            Type stateType,
            CancellationToken token,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null);
        
        #endregion
    }
}