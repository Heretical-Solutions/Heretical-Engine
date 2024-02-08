using System;

namespace HereticalSolutions.Time
{
    //Uncomment attributes for Protobuf and CSV support
    //[Delimiter(",")]
    //[ProtoContract]
    
    /// <summary>
    /// Represents a persistent timer data transfer object (DTO)
    /// </summary>
    [System.Serializable]
    public class PersistentTimerDTO
    {
        //[Name("ID")]
        //[ProtoMember(1)]
        /// <summary>
        /// Gets or sets the ID of the persistent timer
        /// </summary>
        public string ID { get; set; }

        //[Name("State")]
        //[ProtoMember(2)]
        /// <summary>
        /// Gets or sets the state of the persistent timer
        /// </summary>
        public ETimerState State { get; set; }

        //[Name("StartTime")]
        //[ProtoMember(3)]
        /// <summary>
        /// Gets or sets the start time of the persistent timer
        /// </summary>
        public DateTime StartTime { get; set; }

        //[Name("EstimatedFinishTime")]
        //[ProtoMember(4)]
        /// <summary>
        /// Gets or sets the estimated finish time of the persistent timer
        /// </summary>
        public DateTime EstimatedFinishTime { get; set; }

        //[Name("SavedProgress")]
        //[ProtoMember(5)]
        /// <summary>
        /// Gets or sets the saved progress of the persistent timer
        /// </summary>
        public TimeSpan SavedProgress { get; set; }

        //[Name("Accumulate")]
        //[ProtoMember(6)]
        /// <summary>
        /// Gets or sets a value indicating whether the persistent timer should accumulate elapsed time or not
        /// </summary>
        public bool Accumulate { get; set; }

        //[Name("Repeat")]
        //[ProtoMember(7)]
        /// <summary>
        /// Gets or sets a value indicating whether the persistent timer should repeat after reaching the finish time or not
        /// </summary>
        public bool Repeat { get; set; }

        //[Name("CurrentDurationSpan")]
        //[ProtoMember(8)]
        /// <summary>
        /// Gets or sets the current duration of the persistent timer
        /// </summary>
        public TimeSpan CurrentDurationSpan { get; set; }

        //[Name("DefaultDurationSpan")]
        //[ProtoMember(9)]
        /// <summary>
        /// Gets or sets the default duration of the persistent timer
        /// </summary>
        public TimeSpan DefaultDurationSpan { get; set; }
    }
}