using System;

namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Represents the different operations that can be performed on a tracker
    /// </summary>
    public enum ETrackerOperation
    {
        /// <summary>
        /// Sets the tracker value to a specific value
        /// </summary>
        SET_TO_VALUE,
        
        /// <summary>
        /// Increments the tracker value
        /// </summary>
        INCREMENT,
        
        /// <summary>
        /// Decrements the tracker value
        /// </summary>
        DECREMENT,
        
        /// <summary>
        /// Adds a value to the tracker
        /// </summary>
        ADD,
        
        /// <summary>
        /// Subtracts a value from the tracker
        /// </summary>
        SUBSTRACT,
        
        /// <summary>
        /// Multiplies the tracker value by a specific value
        /// </summary>
        MULTIPLY,
        
        /// <summary>
        /// Divides the tracker value by a specific value
        /// </summary>
        DIVIDE
    }
}