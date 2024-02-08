using HereticalSolutions.StateMachines;

namespace HereticalSolutions.StateMachines
{
    /// <summary>
    /// Represents a transition event with rules
    /// </summary>
    /// <typeparam name="TBaseState">The base state type.</typeparam>
    public class TransitionEventWithRules<TBaseState> : ITransitionEvent<TBaseState>
        where TBaseState : IState
    {
        /// <summary>
        /// Gets the state where the transition is from
        /// </summary>
        public TBaseState From { get; protected set; }

        /// <summary>
        /// Gets the state where the transition is to
        /// </summary>
        public TBaseState To { get; protected set; }

        /// <summary>
        /// Gets the asynchronous transition rules
        /// </summary>
        public EAsyncTransitionRules Rules { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionEventWithRules{TBaseState}"/> class
        /// </summary>
        /// <param name="from">The state where the transition is from.</param>
        /// <param name="to">The state where the transition is to.</param>
        /// <param name="rules">The asynchronous transition rules.</param>
        public TransitionEventWithRules(
            TBaseState from,
            TBaseState to,
            EAsyncTransitionRules rules)
        {
            From = from;
            To = to;
            Rules = rules;
        }

        /// <summary>
        /// Returns a string representation of the transition event
        /// </summary>
        /// <returns>A string that represents the transition event.</returns>
        public override string ToString()
        {
            return $"[{From.GetType().Name} => {To.GetType().Name}] ({Rules.ToString()})";
        }
    }
}