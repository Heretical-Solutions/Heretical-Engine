namespace HereticalSolutions.StateMachines
{
    /// <summary>
    /// Represents a basic transition event
    /// </summary>
    /// <typeparam name="TBaseState">The base state type.</typeparam>
    public class BasicTransitionEvent<TBaseState> : ITransitionEvent<TBaseState>
        where TBaseState : IState
    {
        /// <summary>
        /// Gets or sets the state transitioned from
        /// </summary>
        public TBaseState From { get; protected set; }
        
        /// <summary>
        /// Gets or sets the state transitioned to
        /// </summary>
        public TBaseState To { get; protected set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicTransitionEvent{TBaseState}"/> class
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="to">The state to transition to.</param>
        public BasicTransitionEvent(
            TBaseState from,
            TBaseState to)
        {
            From = from;
            
            To = to;
        }
        
        /// <summary>
        /// Returns a string that represents the current transition event
        /// </summary>
        /// <returns>A string that represents the current transition event.</returns>
        public override string ToString()
        {
            return $"[{From.GetType().Name} => {To.GetType().Name}]";
        }
    }
}