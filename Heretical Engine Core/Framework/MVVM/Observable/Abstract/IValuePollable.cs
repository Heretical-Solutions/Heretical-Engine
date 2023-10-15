namespace HereticalSolutions.MVVM
{
    /// <summary>
    /// Interface for objects that can poll a value.
    /// </summary>
    public interface IValuePollable
    {
        /// <summary>
        /// Polls the value.
        /// </summary>
        void PollValue();
    }
}