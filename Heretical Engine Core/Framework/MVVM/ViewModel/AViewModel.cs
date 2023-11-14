using System;
using System.Collections.Generic;

using HereticalSolutions.Logging;

namespace HereticalSolutions.MVVM.ViewModel
{
    /// <summary>
    /// Base class for view models containing boilerplate logic
    /// </summary>
    public abstract class AViewModel : IViewModel
    {
        protected IFormatLogger logger;

        /// <summary>
        /// Constructor for AViewModel class
        /// </summary>
        /// <param name="logger">Logger for logging</param>
        public AViewModel(IFormatLogger logger)
        {
            this.logger = logger;
        }

        #region ILifetimable

        /// <summary>
        /// Determines if the view model is set up
        /// </summary>
        public bool IsSetUp { get; protected set; } = false;
        
        /// <summary>
        /// Determines if the view model is initialized
        /// </summary>
        public bool IsInitialized { get; protected set; } = false;
        
        /// <summary>
        /// Callback for initialization
        /// </summary>
        public Action OnInitialized { get; set; }

        /// <summary>
        /// Callback for cleanup
        /// </summary>
        public Action OnCleanedUp { get; set; }

        /// <summary>
        /// Callback for tear down
        /// </summary>
        public Action OnTornDown { get; set; }

        /// <summary>
        /// Sets up the view model
        /// </summary>
        public virtual void SetUp()
        {
            if (IsSetUp)
                logger?.ThrowException(
                    GetType(),
                    "ALREADY SET UP");
            
            IsSetUp = true;
        }

        /// <summary>
        /// Initializes the view model
        /// </summary>
        public virtual void Initialize()
        {
            if (!IsSetUp)
            {
                logger?.ThrowException(
                    GetType(),
                    "VIEWMODEL SHOULD BE SET UP BEFORE BEING INITIALIZED");
            }

            if (IsInitialized)
            {
                logger?.ThrowException(
                    GetType(),
                    $"INITIALIZING VIEWMODEL THAT IS ALREADY INITIALIZED");
            }

            IsInitialized = true;

            OnInitialized?.Invoke();
        }

        /// <summary>
        /// Cleans up the view model
        /// </summary>
        public virtual void Cleanup()
        {
            if (!IsInitialized)
                return;
            
            IsInitialized = false;

            UnpublishObservables();
            
            UnpublishCommands();
            
            OnCleanedUp?.Invoke();
        }

        /// <summary>
        /// Tears down the view model
        /// </summary>
        public virtual void TearDown()
        {
            if (!IsSetUp)
                return;
            
            IsSetUp = false;
            
            Cleanup();

            OnTornDown?.Invoke();
            
            OnInitialized = null;
            
            OnCleanedUp = null;
            
            OnTornDown = null;
        }

        #endregion

        #region Observables

        /// <summary>
        /// Dictionary to store exposed observables
        /// </summary>
        protected Dictionary<string, object> observables = new Dictionary<string, object>();

        /// <summary>
        /// Adds an observable to the list of exposed observables
        /// </summary>
        /// <param name="key">Identifier for the observable</param>
        /// <param name="observable">Observable object</param>
        protected void PublishObservable(string key, object observable)
        {
            if (observables.ContainsKey(key))
            {
                logger?.ThrowException(
                    GetType(),
                    $"EXPOSED OBSERVABLES LIST ALREADY HAS A KEY \"{key}\"");
            }

            observables.Add(key, observable);
        }

        /// <summary>
        /// Tries to get an observable by its identifier
        /// </summary>
        /// <typeparam name="T">Generic type of the observable</typeparam>
        /// <param name="key">Identifier of the observable</param>
        /// <param name="observable">Observable object</param>
        /// <returns>Whether the observable is present in the exposed observables list</returns>
        public bool GetObservable<T>(string key, out IObservableProperty<T> observable)
        {
            object output;

            bool result = observables.TryGetValue(key, out output);

            observable = result ? (IObservableProperty<T>)output : null;

            return result;
        }
        
        /// <summary>
        /// Tries to poll the value of an observable property using the active poll delegate
        /// </summary>
        /// <param name="observableProperty">Observable property</param>
        /// <typeparam name="T">Value type of the observable property</typeparam>
        protected void TryPollObservable<T>(IObservableProperty<T> observableProperty)
        {
            if (observableProperty != null)
                ((IValuePollable)observableProperty).PollValue();
        }

        /// <summary>
        /// Tears down the observable and cleans it up
        /// </summary>
        /// <param name="observableProperty">Observable property</param>
        /// <typeparam name="T">Value type of the observable property</typeparam>
        protected void TearDownObservable<T>(ref IObservableProperty<T> observableProperty)
        {
            if (observableProperty != null)
            {
                observableProperty.Cleanup();

                observableProperty = null;
            }
        }

        /// <summary>
        /// Cleans up the exposed observables list
        /// </summary>
        protected void UnpublishObservables()
        {
            observables.Clear();
        }
        
        #endregion
        
        #region Commands
        
        /// <summary>
        /// Dictionary to store the delegates that views can fire as callbacks to user actions
        /// </summary>
        protected Dictionary<string, CommandDelegate> commands = new Dictionary<string, CommandDelegate>();

        /// <summary>
        /// Dictionary to store the delegates that views can fire as callbacks to user actions with arguments
        /// </summary>
        protected Dictionary<string, CommandWithArgsDelegate> commandsWithArguments = new Dictionary<string, CommandWithArgsDelegate>();
        
        /// <summary>
        /// Adds a delegate to the list of exposed commands
        /// </summary>
        /// <param name="key">Identifier for the command</param>
        /// <param name="delegate">Delegate object</param>
        protected void PublishCommand(string key, CommandDelegate @delegate)
        {
            if (commands.ContainsKey(key))
            {
                logger?.ThrowException(
                    GetType(),
                    $"EXPOSED COMMANDS LIST ALREADY HAS A KEY \"{key}\"");
            }

            commands.Add(key, @delegate);
        }
        
        /// <summary>
        /// Adds a delegate to the list of exposed commands with arguments
        /// </summary>
        /// <param name="key">Identifier for the command</param>
        /// <param name="delegate">Delegate object</param>
        protected void PublishCommandWithArguments(
            string key,
            CommandWithArgsDelegate @delegate)
        {
            if (commandsWithArguments.ContainsKey(key))
            {
                logger?.ThrowException(
                    GetType(),
                    $"EXPOSED COMMANDS WITH ARGUMENTS LIST ALREADY HAS A KEY \"{key}\"");
            }

            commandsWithArguments.Add(key, @delegate);
        }

        /// <summary>
        /// Tries to get a command by its identifier
        /// </summary>
        /// <param name="key">Identifier of the command</param>
        /// <returns>Command delegate</returns>
        public CommandDelegate GetCommand(string key)
        {
            CommandDelegate result = null;

            if (!commands.TryGetValue(key, out result))
            {
                logger?.ThrowException(
                    GetType(),
                    $"EXPOSED COMMANDS LIST DOES NOT HAVE A KEY \"{key}\"");
            }

            return result;
        }
        
        /// <summary>
        /// Tries to get a command with arguments by its identifier
        /// </summary>
        /// <param name="key">Identifier of the command</param>
        /// <returns>Command delegate</returns>
        public CommandWithArgsDelegate GetCommandWithArguments(string key)
        {
            CommandWithArgsDelegate result = null;

            if (!commandsWithArguments.TryGetValue(key, out result))
            {
                logger?.ThrowException(
                    GetType(),
                    $"EXPOSED COMMANDS WITH ARGUMENTS LIST DOES NOT HAVE A KEY \"{key}\"");
            }

            return result;
        }

        /// <summary>
        /// Cleans up the exposed commands list
        /// </summary>
        protected void UnpublishCommands()
        {
            commands.Clear();
            
            commandsWithArguments.Clear();
        }

        #endregion
    }
}