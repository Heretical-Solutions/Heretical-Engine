using System.Collections.Generic;

namespace HereticalSolutions.Time
{
    /// <summary>
    /// Represents a comparer for comparing <see cref="ETimerState"/> values
    /// </summary>
    public struct ETimerStateComparer : IEqualityComparer<ETimerState>
    {
        /// <summary>
        /// Determines whether two <see cref="ETimerState"/> values are equal
        /// </summary>
        /// <param name="stateA">The first <see cref="ETimerState"/> to compare.</param>
        /// <param name="stateB">The second <see cref="ETimerState"/> to compare.</param>
        /// <returns><c>true</c> if both <see cref="ETimerState"/> values are equal, otherwise <c>false</c>.</returns>
        public bool Equals(ETimerState stateA, ETimerState stateB)
        {
            return stateA == stateB;
        }

        /// <summary>
        /// Returns the hash code for a <see cref="ETimerState"/> value
        /// </summary>
        /// <param name="state">The <see cref="ETimerState"/> for which to get the hash code.</param>
        /// <returns>The hash code for the specified <see cref="ETimerState"/> value.</returns>
        public int GetHashCode(ETimerState state)
        {
            // you need to do some thinking here,
            return (int)state;
        }
    }
}