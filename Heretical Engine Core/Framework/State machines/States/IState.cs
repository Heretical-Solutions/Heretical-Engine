namespace HereticalSolutions.StateMachines
{
    /// <summary>
    /// Represents an interface for a state in a state machine
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Called when the state is entered
        /// </summary>
        void OnStateEntered();
        
        /// <summary>
        /// Called when the state is exited
        /// </summary>
        void OnStateExited();
    }
}