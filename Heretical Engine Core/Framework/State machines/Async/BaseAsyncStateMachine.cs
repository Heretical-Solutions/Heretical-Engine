using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HereticalSolutions.Logging;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.StateMachines
{
    /// <summary>
    /// Represents a base asynchronous state machine implementation
    /// </summary>
    /// <typeparam name="TBaseState">The base state type.</typeparam>
    public class BaseAsyncStateMachine<TBaseState> : IAsyncStateMachine<TBaseState>
        where TBaseState : IState
    {
        private readonly IReadOnlyRepository<Type, TBaseState> states;

        private readonly IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events;

        private readonly Queue<TransitionRequest<TBaseState>> transitionRequestsQueue;

        private readonly ITransitionController<TBaseState> transitionController;

        private readonly EAsyncTransitionRules defaultAsyncTransitionRules;

        private readonly ILogger logger;


        private Task processTransitionQueueTask;

        private readonly object lockObject;

        private CancellationTokenSource processTransitionQueueCancellationTokenSource;

        private CancellationTokenSource transitToImmediatelyCancellationTokenSource;

        private bool transitionInProgress;


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAsyncStateMachine{TBaseState}"/> class
        /// </summary>
        /// <param name="states">The repository of all states.</param>
        /// <param name="events">The repository of all transition events.</param>
        /// <param name="transitionRequestsQueue">The queue of transition requests.</param>
        /// <param name="transitionController">The transition controller.</param>
        /// <param name="defaultAsyncTransitionRules">The default asynchronous transition rules.</param>
        /// <param name="currentState">The current state of the state machine.</param>
        /// <param name="logger">The logger.</param>
        public BaseAsyncStateMachine(
            IReadOnlyRepository<Type, TBaseState> states,
            IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events,
            Queue<TransitionRequest<TBaseState>> transitionRequestsQueue,
            ITransitionController<TBaseState> transitionController,
            EAsyncTransitionRules defaultAsyncTransitionRules,
            TBaseState currentState,
            ILogger logger = null)
        {
            this.states = states;

            this.events = events;

            this.transitionRequestsQueue = transitionRequestsQueue;

            this.transitionController = transitionController;

            this.defaultAsyncTransitionRules = defaultAsyncTransitionRules;

            this.logger = logger;


            CurrentState = currentState;

            OnCurrentStateChangeStarted = null;

            OnCurrentStateChangeFinished = null;

            transitionInProgress = false;

            lockObject = new object();
        }

        #region IAsyncStateMachine

        /// <summary>
        /// Gets a value indicating whether a transition is currently in progress
        /// </summary>
        public bool TransitionInProgress => transitionInProgress;

        #region Current state

        /// <summary>
        /// Gets or sets the current state of the state machine
        /// </summary>
        public TBaseState CurrentState { get; private set; }

        /// <summary>
        /// Gets or sets the action invoked when the current state change is started
        /// </summary>
        public Action<TBaseState, TBaseState> OnCurrentStateChangeStarted { get; set; }

        /// <summary>
        /// Gets or sets the action invoked when the current state change is finished
        /// </summary>
        public Action<TBaseState, TBaseState> OnCurrentStateChangeFinished { get; set; }

        #endregion

        #region All states

        /// <summary>
        /// Gets the state of the specified concrete type
        /// </summary>
        /// <typeparam name="TConcreteState">The concrete state type.</typeparam>
        /// <returns>The state instance.</returns>
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
        /// <returns>The state instance.</returns>
        public TBaseState GetState(Type stateType)
        {
            if (!states.TryGet(stateType, out var result))
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"STATE {stateType.Name} NOT FOUND"));

            return result;
        }

        /// <summary>
        /// Gets all the types of states in the state machine
        /// </summary>
        public IEnumerable<Type> AllStates => states.Keys;

        #endregion

        #region Event handling

        /// <summary>
        /// Handles the specified event asynchronously
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to handle.</typeparam>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Handle<TEvent>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null)
        {
            ITransitionEvent<TBaseState> @event;

            if (!events.TryGet(typeof(TEvent), out @event))
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"EVENT {typeof(TEvent).Name} NOT FOUND"));

            var request = new TransitionRequest<TBaseState>(
                @event,
                new CancellationTokenSource(),
                stateExitProgress,
                stateEnterProgress,
                protocol);

            lock (lockObject)
            {
                transitionRequestsQueue.Enqueue(request);

                if (processTransitionQueueTask == null
                    || processTransitionQueueTask.IsCompleted)
                {
                    processTransitionQueueCancellationTokenSource?.Dispose();

                    processTransitionQueueCancellationTokenSource = new CancellationTokenSource();

                    processTransitionQueueTask = ProcessTransitionQueueTask(processTransitionQueueCancellationTokenSource.Token);
                }
            }

            while (request.TransitionState != EAsyncTransitionState.ABORTED
                   && request.TransitionState != EAsyncTransitionState.COMPLETED)
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// Handles the specified event type asynchronously
        /// </summary>
        /// <param name="eventType">The type of the event to handle.</param>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Handle(
            Type eventType,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null)
        {
            ITransitionEvent<TBaseState> @event;

            if (!events.TryGet(eventType, out @event))
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"EVENT {eventType.Name} NOT FOUND"));

            var request = new TransitionRequest<TBaseState>(
                @event,
                new CancellationTokenSource(),
                stateExitProgress,
                stateEnterProgress,
                protocol);

            lock (lockObject)
            {
                transitionRequestsQueue.Enqueue(request);

                if (processTransitionQueueTask == null
                    || processTransitionQueueTask.IsCompleted)
                {
                    processTransitionQueueCancellationTokenSource?.Dispose();

                    processTransitionQueueCancellationTokenSource = new CancellationTokenSource();

                    processTransitionQueueTask = ProcessTransitionQueueTask(processTransitionQueueCancellationTokenSource.Token);
                }
            }

            while (request.TransitionState != EAsyncTransitionState.ABORTED
                   && request.TransitionState != EAsyncTransitionState.COMPLETED)
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// Handles the specified event asynchronously with a cancellation token
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to handle.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Handle<TEvent>(
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null)
        {
            ITransitionEvent<TBaseState> @event;

            if (!events.TryGet(typeof(TEvent), out @event))
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"EVENT {typeof(TEvent).Name} NOT FOUND"));

            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var request = new TransitionRequest<TBaseState>(
                @event,
                cancellationTokenSource,
                stateExitProgress,
                stateEnterProgress,
                protocol);

            lock (lockObject)
            {
                transitionRequestsQueue.Enqueue(request);

                if (processTransitionQueueTask == null
                    || processTransitionQueueTask.IsCompleted)
                {
                    processTransitionQueueCancellationTokenSource?.Dispose();

                    processTransitionQueueCancellationTokenSource = new CancellationTokenSource();

                    processTransitionQueueTask = ProcessTransitionQueueTask(processTransitionQueueCancellationTokenSource.Token);
                }
            }

            while (request.TransitionState != EAsyncTransitionState.ABORTED
                   && request.TransitionState != EAsyncTransitionState.COMPLETED)
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// Handles the specified event type asynchronously with a cancellation token
        /// </summary>
        /// <param name="eventType">The type of the event to handle.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Handle(
            Type eventType,
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null)
        {
            ITransitionEvent<TBaseState> @event;

            if (!events.TryGet(eventType, out @event))
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"EVENT {eventType.Name} NOT FOUND"));

            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var request = new TransitionRequest<TBaseState>(
                @event,
                cancellationTokenSource,
                stateExitProgress,
                stateEnterProgress,
                protocol);

            lock (lockObject)
            {
                transitionRequestsQueue.Enqueue(request);

                if (processTransitionQueueTask == null
                    || processTransitionQueueTask.IsCompleted)
                {
                    processTransitionQueueCancellationTokenSource?.Dispose();

                    processTransitionQueueCancellationTokenSource = new CancellationTokenSource();

                    processTransitionQueueTask = ProcessTransitionQueueTask(processTransitionQueueCancellationTokenSource.Token);
                }
            }

            while (request.TransitionState != EAsyncTransitionState.ABORTED
                   && request.TransitionState != EAsyncTransitionState.COMPLETED)
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// Gets or sets the action invoked when an event is fired
        /// </summary>
        public Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }

        #endregion

        #region Immediate transition

        /// <summary>
        /// Transitions immediately to the specified state type asynchronously
        /// </summary>
        /// <typeparam name="TState">The type of the state to transition to.</typeparam>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TransitToImmediately<TState>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null)
        {
            if (!states.Has(typeof(TState)))
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"STATE {typeof(TState).Name} NOT FOUND"));

            var previousState = CurrentState;

            var newState = states.Get(typeof(TState));

            ClearTransitionQueue();

            CancelImmediateTransitions();

            using (transitToImmediatelyCancellationTokenSource = new CancellationTokenSource())
            {
                try
                {
                    await PerformTransition(
                        previousState,
                        newState,
                        defaultAsyncTransitionRules,
                        transitToImmediatelyCancellationTokenSource.Token,
                        stateExitProgress,
                        stateEnterProgress,
                        protocol)
                        .ThrowExceptions(
                            GetType(),
                            logger);
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Transitions immediately to the specified state type asynchronously
        /// </summary>
        /// <param name="stateType">The type of the state to transition to.</param>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TransitToImmediately(
            Type stateType,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null)
        {
            if (!states.Has(stateType))
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"STATE {stateType.Name} NOT FOUND"));

            var previousState = CurrentState;

            var newState = states.Get(stateType);

            ClearTransitionQueue();

            CancelImmediateTransitions();

            using (transitToImmediatelyCancellationTokenSource = new CancellationTokenSource())
            {
                try
                {
                    await PerformTransition(
                        previousState,
                        newState,
                        defaultAsyncTransitionRules,
                        transitToImmediatelyCancellationTokenSource.Token,
                        stateExitProgress,
                        stateEnterProgress,
                        protocol)
                        .ThrowExceptions(
                            GetType(),
                            logger);
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Transitions immediately to the specified state type asynchronously with a cancellation token
        /// </summary>
        /// <typeparam name="TState">The type of the state to transition to.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TransitToImmediately<TState>(
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null)
        {
            if (!states.Has(typeof(TState)))
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"STATE {typeof(TState).Name} NOT FOUND"));

            var previousState = CurrentState;

            var newState = states.Get(typeof(TState));

            ClearTransitionQueue();

            CancelImmediateTransitions();

            using (transitToImmediatelyCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                try
                {
                    await PerformTransition(
                        previousState,
                        newState,
                        defaultAsyncTransitionRules,
                        transitToImmediatelyCancellationTokenSource.Token,
                        stateExitProgress,
                        stateEnterProgress,
                        protocol)
                        .ThrowExceptions(
                            GetType(),
                            logger);
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Transitions immediately to the specified state type asynchronously with a cancellation token
        /// </summary>
        /// <param name="stateType">The type of the state to transition to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TransitToImmediately(
            Type stateType,
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null)
        {
            if (!states.Has(stateType))
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"STATE {stateType.Name} NOT FOUND"));

            var previousState = CurrentState;

            var newState = states.Get(stateType);

            ClearTransitionQueue();

            CancelImmediateTransitions();

            using (transitToImmediatelyCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                try
                {
                    await PerformTransition(
                        previousState,
                        newState,
                        defaultAsyncTransitionRules,
                        transitToImmediatelyCancellationTokenSource.Token,
                        stateExitProgress,
                        stateEnterProgress,
                        protocol)
                        .ThrowExceptions(
                            GetType(),
                            logger);
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

        #endregion

        #endregion

        private async Task ProcessTransitionQueueTask(
            CancellationToken cancellationToken)
        {
            transitionInProgress = true;

            while (
                transitionRequestsQueue.Count > 0
                && !cancellationToken.IsCancellationRequested)
            {
                TransitionRequest<TBaseState> nextRequest;

                lock (lockObject) 
                {
                    nextRequest = transitionRequestsQueue.Dequeue();
                }

                nextRequest.TransitionState = EAsyncTransitionState.IN_PROGRESS;

                using (CancellationTokenSource combinedTokenSource =
                       CancellationTokenSource.CreateLinkedTokenSource(
                           nextRequest.CancellationTokenSource.Token,
                           cancellationToken))
                {
                    try
                    {
                        await PerformTransition(
                            nextRequest.Event,
                            nextRequest.CancellationTokenSource.Token,
                            nextRequest.StateExitProgress,
                            nextRequest.StateEnterProgress,
                            nextRequest.TransitionProtocol)
                            .ThrowExceptions(
                                GetType(),
                                logger);
                    }
                    catch (Exception e)
                    {
                        nextRequest.TransitionState = EAsyncTransitionState.ABORTED;
                    }
                    finally
                    {
                        if (combinedTokenSource.Token.IsCancellationRequested)
                        {
                            nextRequest.TransitionState = EAsyncTransitionState.ABORTED;
                        }
                        else
                        {
                            nextRequest.TransitionState = EAsyncTransitionState.COMPLETED;
                        }

                        combinedTokenSource?.Dispose();
                    }
                }
                
                nextRequest.CancellationTokenSource?.Dispose();
            }

            transitionInProgress = false;
        }
        
        private void ClearTransitionQueue()
        {
            processTransitionQueueCancellationTokenSource?.Dispose();
            
            processTransitionQueueCancellationTokenSource?.Cancel();
            
            lock (lockObject)
            {
                foreach (var request in transitionRequestsQueue)
                {
                    request.TransitionState = EAsyncTransitionState.ABORTED;
                }

                transitionRequestsQueue.Clear();
            }
        }

        private void CancelImmediateTransitions()
        {
            transitToImmediatelyCancellationTokenSource?.Dispose();
            
            transitToImmediatelyCancellationTokenSource?.Cancel();
        }
        
        private async Task PerformTransition(
            ITransitionEvent<TBaseState> @event,
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null)
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

            var rules = defaultAsyncTransitionRules;

            if (@event is TransitionEventWithRules<TBaseState>)
                rules = ((TransitionEventWithRules<TBaseState>)@event).Rules;

            await PerformTransition(
                previousState,
                newState,
                rules,
                cancellationToken,
                stateExitProgress,
                stateEnterProgress,
                protocol)
                .ThrowExceptions(
                    GetType(),
                    logger);
        }

        private async Task PerformTransition(
            TBaseState previousState,
            TBaseState newState,
            EAsyncTransitionRules rules,
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null)
        {
            try
            {
                switch (rules)
                {
                    case EAsyncTransitionRules.TRANSIT_SEQUENTIALLY_UNLOAD_THEN_LOAD:

                        OnCurrentStateChangeStarted?.Invoke(
                            previousState,
                            newState);

                        await ExitState(
                            previousState,
                            cancellationToken,
                            stateExitProgress,
                            protocol)
                            .ThrowExceptions(
                                GetType(),
                                logger);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        CurrentState = newState;

                        await EnterState(
                            newState,
                            cancellationToken,
                            stateEnterProgress,
                            protocol)
                            .ThrowExceptions(
                                GetType(),
                                logger);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        
                        OnCurrentStateChangeFinished?.Invoke(previousState, newState);

                        break;

                    case EAsyncTransitionRules.TRANSIT_SEQUENTIALLY_LOAD_THEN_UNLOAD:
                        
                        OnCurrentStateChangeStarted?.Invoke(
                            previousState,
                            newState);

                        await EnterState(
                            newState,
                            cancellationToken,
                            stateEnterProgress,
                            protocol)
                            .ThrowExceptions(
                                GetType(),
                                logger);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        
                        CurrentState = newState;

                        await ExitState(
                            previousState,
                            cancellationToken,
                            stateExitProgress,
                            protocol)
                            .ThrowExceptions(
                                GetType(),
                                logger);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        OnCurrentStateChangeFinished?.Invoke(previousState, newState);

                        break;

                    case EAsyncTransitionRules.TRANSIT_SIMULTANEOUSLY:

                        OnCurrentStateChangeStarted?.Invoke(previousState, newState);

                        await Task
                            .WhenAll(
                                EnterState(
                                    newState,
                                    cancellationToken,
                                    stateEnterProgress,
                                    protocol),
                                ExitState(
                                    previousState,
                                    cancellationToken,
                                    stateExitProgress,
                                    protocol))
                            .ThrowExceptions(
                                GetType(),
                                logger);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        CurrentState = newState;

                        OnCurrentStateChangeFinished?.Invoke(previousState, newState);

                        break;
                }
            }
            catch (OperationCanceledException)
            {
                //BOING
            }
        }

        private async Task ExitState(
            TBaseState previousState,
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            TransitionProtocol protocol = null)
        {
            if (protocol != null)
            {
                while (protocol.CommencePreviousStateExitStart != true)
                {
                    await Task.Yield();
                                
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }

            await transitionController.ExitState(
                previousState,
                cancellationToken,
                stateExitProgress)
                .ThrowExceptions(
                    GetType(),
                    logger);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
                        
            previousState.OnStateExited();

            protocol?.OnPreviousStateExited?.Invoke(previousState);
                        
            if (protocol != null)
            {
                while (protocol.CommencePreviousStateExitFinish != true)
                {
                    await Task.Yield();
                                
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
        }

        private async Task EnterState(
            TBaseState newState,
            CancellationToken cancellationToken,
            IProgress<float> stateEnterProgress = null,
            TransitionProtocol protocol = null)
        {
            if (protocol != null)
            {
                while (protocol.CommenceNextStateEnterStart != true)
                {
                    await Task.Yield();
                                
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
                        
            await transitionController.EnterState(
                newState,
                cancellationToken,
                stateEnterProgress)
                .ThrowExceptions(
                    GetType(),
                    logger);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            newState.OnStateEntered();
                        
            protocol?.OnNextStateEntered?.Invoke(newState);
                        
            if (protocol != null)
            {
                while (protocol.CommenceNextStateEnterFinish != true)
                {
                    await Task.Yield();
                                
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
        }
    }
}