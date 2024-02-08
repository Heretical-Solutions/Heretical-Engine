using System;

namespace HereticalSolutions.StateMachines
{
    /// <summary>
    /// Represents a transition protocol
    /// </summary>
    public class TransitionProtocol
    {
        /// <summary>
        /// Gets or sets a value indicating whether the previous state exit start should commence
        /// </summary>
        public bool CommencePreviousStateExitStart { get; set; }

        /// <summary>
        /// Gets or sets the action to be performed when the previous state is exited
        /// </summary>
        public Action<IState> OnPreviousStateExited { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the previous state exit finish should commence
        /// </summary>
        public bool CommencePreviousStateExitFinish { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the next state enter start should commence
        /// </summary>
        public bool CommenceNextStateEnterStart { get; set; }

        /// <summary>
        /// Gets or sets the action to be performed when the next state is entered
        /// </summary>
        public Action<IState> OnNextStateEntered { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the next state enter finish should commence
        /// </summary>
        public bool CommenceNextStateEnterFinish { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionProtocol"/> class
        /// </summary>
        public TransitionProtocol()
        {
            CommencePreviousStateExitStart = true;

            CommencePreviousStateExitFinish = true;
        
            CommenceNextStateEnterStart = true;

            CommenceNextStateEnterFinish = true;
        }
    }
}