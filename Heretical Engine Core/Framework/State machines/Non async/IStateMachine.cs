using System;
using System.Collections.Generic;

namespace HereticalSolutions.StateMachines
{
    /// <summary>
    /// Represents a state machine interface
    /// </summary>
    /// <typeparam name="TBaseState">The base state type of the state machine.</typeparam>
    public interface IStateMachine<TBaseState>
        where TBaseState : IState
    {
        /// <summary>
        /// Gets a value indicating whether a state transition is in progress
        /// </summary>
        bool TransitionInProgress { get; }
        
        #region Current state

        /// <summary>
        /// Gets or sets the current state of the state machine
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
        /// Gets an instance of the specified concrete state
        /// </summary>
        /// <typeparam name="TConcreteState">The concrete state type.</typeparam>
        /// <returns>An instance of the specified concrete state.</returns>
        TBaseState GetState<TConcreteState>();

        /// <summary>
        /// Gets an instance of the specified state type
        /// </summary>
        /// <param name="stateType">The state type.</param>
        /// <returns>An instance of the specified state type.</returns>
        TBaseState GetState(Type stateType);

        /// <summary>
        /// Gets all the state types used in the state machine
        /// </summary>
        IEnumerable<Type> AllStates { get; }

        #endregion
        
        #region Event handling
        
        /// <summary>
        /// Handles the specified event type
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        void Handle<TEvent>();
        
        /// <summary>
        /// Handles the specified event type
        /// </summary>
        /// <param name="eventType">The event type.</param>
        void Handle(Type eventType);
        
        /// <summary>
        /// Gets or sets the action to be executed when an event is fired
        /// </summary>
        Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }
        
        #endregion

        #region Immediate transition
        
        /// <summary>
        /// Transitions to the specified state immediately
        /// </summary>
        /// <typeparam name="TState">The state type to transition to.</typeparam>
        void TransitToImmediately<TState>();
        
        /// <summary>
        /// Transitions to the specified state type immediately
        /// </summary>
        /// <param name="stateType">The state type to transition to.</param>
        void TransitToImmediately(Type stateType);
        
        #endregion
    }
}