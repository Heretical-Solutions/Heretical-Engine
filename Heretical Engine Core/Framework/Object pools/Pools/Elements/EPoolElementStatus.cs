namespace HereticalSolutions.Pools
{
    ///<summary>
    /// Enumeration representing the status of an element in a pool.
    ///</summary>
    public enum EPoolElementStatus
    {
        ///<summary>
        /// The element is uninitialized.
        ///</summary>
        UNINITIALIZED,
        ///<summary>
        /// The element has been popped from the pool.
        ///</summary>
        POPPED,
        ///<summary>
        /// The element has been pushed back into the pool.
        ///</summary>
        PUSHED
    }
}