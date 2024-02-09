namespace HereticalSolutions.Quests
{
    ///<summary>
    /// Represents the different types of comparisons that can be used in a quest tracker
    ///</summary>
    public enum ETrackerComparison
    {
        ///<summary>
        /// Specifies that the comparison is for equality
        ///</summary>
        EQUALS_TO,
        
        ///<summary>
        /// Specifies that the comparison is for inequality
        ///</summary>
        NOT_EQUALS_TO,
        
        ///<summary>
        /// Specifies that the comparison is for a value greater than another value
        ///</summary>
        MORE_THAN,
        
        ///<summary>
        /// Specifies that the comparison is for a value lesser than another value
        ///</summary>
        LESS_THAN,
        
        ///<summary>
        /// Specifies that the comparison is for a value greater than or equal to another value
        ///</summary>
        EQUALS_OR_MORE_THAN,
        
        ///<summary>
        /// Specifies that the comparison is for a vlaue lesser than or equal to another value
        ///</summary>
        EQUALS_OR_LESS_THAN
    }
}