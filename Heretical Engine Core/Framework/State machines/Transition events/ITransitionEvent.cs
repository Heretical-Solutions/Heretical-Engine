namespace HereticalSolutions.StateMachines
{
    /// <summary>
    /// Represents a transition event in a state machine
    /// </summary>
    /// <typeparam name="TBaseState">The base state type.</typeparam>
    public interface ITransitionEvent<TBaseState>
        where TBaseState : IState
    {
        /// <summary>
        /// Gets the starting state of the transition
        /// </summary>
        TBaseState From { get; }
        
        /// <summary>
        /// Gets the target state of the transition
        /// </summary>
        TBaseState To { get; }
    }
}