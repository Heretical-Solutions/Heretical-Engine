using System;
using System.Collections.Generic;

using HereticalSolutions.Logging;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.StateMachines.Factories
{
    /// <summary>
    /// Provides methods for building state machines
    /// </summary>
    public static class StateMachinesFactory
    {
        /// <summary>
        /// Builds a new instance of the BaseAsyncStateMachine class
        /// </summary>
        /// <typeparam name="TBaseState">The base state type.</typeparam>
        /// <param name="states">The repository of states.</param>
        /// <param name="events">The repository of transition events.</param>
        /// <param name="transitionController">The transition controller.</param>
        /// <param name="asyncTransitionRules">The async transition rules.</param>
        /// <param name="currentState">The current state.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A new instance of the BaseAsyncStateMachine class.</returns>
        public static BaseAsyncStateMachine<TBaseState> BuildBaseAsyncStateMachine<TBaseState>(
            IReadOnlyRepository<Type, TBaseState> states,
            IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events,
            ITransitionController<TBaseState> transitionController,
            EAsyncTransitionRules asyncTransitionRules,
            TBaseState currentState,
            ILoggerResolver loggerResolver = null)
            where TBaseState : IState
        {
            ILogger logger =
                loggerResolver?.GetLogger<BaseAsyncStateMachine<TBaseState>>()
                ?? null;

            return new BaseAsyncStateMachine<TBaseState>(
                states,
                events,
                new Queue<TransitionRequest<TBaseState>>(),
                transitionController,
                asyncTransitionRules,
                currentState,
                logger);
        }
        
        /// <summary>
        /// Builds a new instance of the BaseStateMachine class
        /// </summary>
        /// <typeparam name="TBaseState">The base state type.</typeparam>
        /// <param name="states">The repository of states.</param>
        /// <param name="events">The repository of transition events.</param>
        /// <param name="currentState">The current state.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A new instance of the BaseStateMachine class.</returns>
        public static BaseStateMachine<TBaseState> BuildBaseStateMachine<TBaseState>(
            IReadOnlyRepository<Type, TBaseState> states,
            IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events,
            TBaseState currentState,
            ILoggerResolver loggerResolver = null)
            where TBaseState : IState
        {
            ILogger logger =
                loggerResolver?.GetLogger<BaseStateMachine<TBaseState>>()
                ?? null;

            return new BaseStateMachine<TBaseState>(
                states,
                events,
                new Queue<ITransitionEvent<TBaseState>>(),
                currentState,
                logger);
        }

        /// <summary>
        /// Adds a new state to the repository of states
        /// </summary>
        /// <typeparam name="TBaseState">The base state type.</typeparam>
        /// <typeparam name="T">The state type.</typeparam>
        /// <param name="states">The repository of states.</param>
        /// <returns>The newly added state.</returns>
        public static T AddState<TBaseState, T>(
            IRepository<Type, TBaseState> states)
            where TBaseState : IState
            where T : TBaseState
        {
            var state = (T)Activator.CreateInstance(
                typeof(T));
            
            states.Add(typeof(T), (TBaseState)state);

            return state;
        }
        
        /// <summary>
        /// Adds a new state with arguments to the repository of states
        /// </summary>
        /// <typeparam name="TBaseState">The base state type.</typeparam>
        /// <typeparam name="T">The state type.</typeparam>
        /// <param name="states">The repository of states.</param>
        /// <param name="arguments">The arguments to pass to the state constructor.</param>
        /// <returns>The newly added state.</returns>
        public static T AddStateWithArguments<TBaseState, T>(
            IRepository<Type, TBaseState> states,
            object[] arguments)
            where TBaseState : IState
            where T : TBaseState
        {
            var state = (T)Activator.CreateInstance(
                typeof(T),
                arguments);
            
            states.Add(typeof(T), (TBaseState)state);

            return state;
        }
        
        /// <summary>
        /// Adds a new transition event to the repository of transition events
        /// </summary>
        /// <typeparam name="TBaseState">The base state type.</typeparam>
        /// <typeparam name="TEvent">The transition event type.</typeparam>
        /// <typeparam name="TFrom">The source state type.</typeparam>
        /// <typeparam name="TTo">The target state type.</typeparam>
        /// <param name="states">The repository of states.</param>
        /// <param name="transitionEvents">The repository of transition events.</param>
        /// <returns>The newly added transition event.</returns>
        public static TEvent AddTransitionEvent<TBaseState, TEvent, TFrom, TTo>(
            IRepository<Type, TBaseState> states,
            IRepository<Type, ITransitionEvent<TBaseState>> transitionEvents)
            where TBaseState : IState
            where TEvent : ITransitionEvent<TBaseState>
            where TFrom : TBaseState
            where TTo : TBaseState
        {
            var transitionEvent = (TEvent)Activator.CreateInstance(
                typeof(TEvent),
                new object[]
                {
                    states.Get(typeof(TFrom)),
                    states.Get(typeof(TTo)),
                });

            transitionEvents.Add(typeof(TEvent), transitionEvent);

            return transitionEvent;
        }
    }
}