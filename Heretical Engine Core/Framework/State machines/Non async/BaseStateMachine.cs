	using System;
	using System.Collections.Generic;

	using HereticalSolutions.Logging;
	using HereticalSolutions.Repositories;

	namespace HereticalSolutions.StateMachines
	{
		/// <summary>
		/// Represents a base state machine
		/// </summary>
		/// <typeparam name="TBaseState">The base state type.</typeparam>
		public class BaseStateMachine<TBaseState> : IStateMachine<TBaseState>
			where TBaseState : IState
		{
			private readonly IReadOnlyRepository<Type, TBaseState> states;

			private readonly IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events;

			private readonly Queue<ITransitionEvent<TBaseState>> transitionQueue;

			private readonly ILogger logger;

			/// <summary>
			/// Initializes a new instance of the <see cref="BaseStateMachine{TBaseState}"/> class
			/// </summary>
			/// <param name="states">The repository of states.</param>
			/// <param name="events">The repository of events.</param>
			/// <param name="transitionQueue">The queue of transition events.</param>
			/// <param name="currentState">The current state of the state machine.</param>
			/// <param name="logger">The logger used for logging.</param>
			public BaseStateMachine(
				IReadOnlyRepository<Type, TBaseState> states,
				IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events,
				Queue<ITransitionEvent<TBaseState>> transitionQueue,
				TBaseState currentState,
				ILogger logger = null)
			{
				this.states = states;
				this.events = events;
				this.transitionQueue = transitionQueue;
				this.logger = logger;

				CurrentState = currentState;

				OnCurrentStateChangeStarted = null;
				OnCurrentStateChangeFinished = null;
			}

			#region IStateMachine

			/// <summary>
			/// Gets a value indicating whether a transition is in progress
			/// </summary>
			public bool TransitionInProgress { get; private set; }

			#region Current state

			/// <summary>
			/// Gets or sets the current state of the state machine
			/// </summary>
			public TBaseState CurrentState { get; private set; }

			/// <summary>
			/// Gets or sets the action to perform when the current state change is started
			/// </summary>
			public Action<TBaseState, TBaseState> OnCurrentStateChangeStarted { get; set; }

			/// <summary>
			/// Gets or sets the action to perform when the current state change is finished
			/// </summary>
			public Action<TBaseState, TBaseState> OnCurrentStateChangeFinished { get; set; }

			#endregion

			#region All states

			/// <summary>
			/// Gets all the types of states in the state machine
			/// </summary>
			public IEnumerable<Type> AllStates
			{
				get => states.Keys;
			}

			/// <summary>
			/// Gets the state of the specified type
			/// </summary>
			/// <typeparam name="TConcreteState">The concrete state type.</typeparam>
			/// <returns>The state of the specified type.</returns>
			public TBaseState GetState<TConcreteState>()
			{
				if (!states.TryGet(typeof(TConcreteState), out var result))
					throw new Exception(
						logger.TryFormat(
							GetType(),
							$"STATE {typeof(TConcreteState).Name} NOT FOUND"));

				return result;
			}

			/// <summary>
			/// Gets the state of the specified type
			/// </summary>
			/// <param name="stateType">The type of the state.</param>
			/// <returns>The state of the specified type.</returns>
			public TBaseState GetState(Type stateType)
			{
				if (!states.TryGet(stateType, out var result))
					throw new Exception(
						logger.TryFormat(
							GetType(),
							$"STATE {stateType.Name} NOT FOUND"));

				return result;
			}

			#endregion

			#region Event handling

			/// <summary>
			/// Handles the specified event
			/// </summary>
			/// <typeparam name="TEvent">The type of the event.</typeparam>
			public void Handle<TEvent>()
			{
				ITransitionEvent<TBaseState> @event;

				if (!events.TryGet(typeof(TEvent), out @event))
					throw new Exception(
						logger.TryFormat(
							GetType(),
							$"EVENT {typeof(TEvent).Name} NOT FOUND"));

				if (TransitionInProgress)
					transitionQueue.Enqueue(@event);
				else
					PerformTransition(@event);
			}

			/// <summary>
			/// Handles the specified event
			/// </summary>
			/// <param name="eventType">The type of the event.</param>
			public void Handle(Type eventType)
			{
				ITransitionEvent<TBaseState> @event;

				if (!events.TryGet(eventType, out @event))
					throw new Exception(
						logger.TryFormat(
							GetType(),
							$"EVENT {eventType.Name} NOT FOUND"));

				if (TransitionInProgress)
					transitionQueue.Enqueue(@event);
				else
					PerformTransition(@event);
			}

			/// <summary>
			/// Gets or sets the action to perform when an event is fired
			/// </summary>
			public Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }

			#endregion

			#region Immediate transition

			/// <summary>
			/// Transitions immediately to the specified state
			/// </summary>
			/// <typeparam name="TState">The type of the state.</typeparam>
			public void TransitToImmediately<TState>()
			{
				if (!states.Has(typeof(TState)))
					throw new Exception(
						logger.TryFormat(
							GetType(),
							$"STATE {typeof(TState).Name} NOT FOUND"));

				var previousState = CurrentState;
				var newState = states.Get(typeof(TState));

				PerformTransition(
					previousState,
					newState);
			}

			/// <summary>
			/// Transitions immediately to the specified state
			/// </summary>
			/// <param name="stateType">The type of the state.</param>
			public void TransitToImmediately(Type stateType)
			{
				if (!states.Has(stateType))
					throw new Exception(
						logger.TryFormat(
							GetType(),
							$"STATE {stateType.Name} NOT FOUND"));

				var previousState = CurrentState;
				var newState = states.Get(stateType);

				PerformTransition(
					previousState,
					newState);
			}

			#endregion

			#endregion

			private void PerformTransition(ITransitionEvent<TBaseState> @event)
			{
				if (!EqualityComparer<TBaseState>.Default.Equals(CurrentState, @event.From))
				{
					string currentStateString = CurrentState.GetType().Name;
					string fromStateString = @event.From.GetType().Name;

					throw new Exception(
						logger.TryFormat(
							GetType(),
							$"CURRENT STATE {currentStateString} IS NOT EQUAL TO TRANSITION FROM STATE {fromStateString}"));
				}

				OnEventFired?.Invoke(@event);

				var previousState = CurrentState;
				var newState = @event.To;

				PerformTransition(previousState, newState);
			}

			private void PerformTransition(
				TBaseState previousState,
				TBaseState newState)
			{
				TransitionInProgress = true;

				OnCurrentStateChangeStarted?.Invoke(previousState, newState);

				previousState.OnStateExited();

				CurrentState = newState;

				newState.OnStateEntered();

				OnCurrentStateChangeFinished?.Invoke(previousState, newState);

				TransitionInProgress = false;

				if (transitionQueue.Count != 0)
				{
					PerformTransition(transitionQueue.Dequeue());
				}
			}
		}
	}