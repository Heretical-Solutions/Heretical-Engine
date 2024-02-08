// Uncomment usings for Protobuf and CSV support
// using CsvHelper.Configuration.Attributes;
// using ProtoBuf;

namespace HereticalSolutions.Time
{
    // Uncomment attributes for Protobuf and CSV support
    // [Delimiter(",")]
    // [ProtoContract]
    
    /// <summary>
    /// Represents a data transfer object for the runtime timer
    /// </summary>
    [System.Serializable]
    public class RuntimeTimerDTO
    {
        // [Name("ID")]
        // [ProtoMember(1)]
        /// <summary>
        /// Gets or sets the ID of the timer
        /// </summary>
        public string ID { get; set; }

        // [Name("State")]
        // [ProtoMember(2)]
        /// <summary>
        /// Gets or sets the current state of the timer
        /// </summary>
        public ETimerState State { get; set; }

        // [Name("CurrentTimeElapsed")]
        // [ProtoMember(3)]
        /// <summary>
        /// Gets or sets the current time elapsed for the timer
        /// </summary>
        public float CurrentTimeElapsed { get; set; }
        
        // [Name("Accumulate")]
        // [ProtoMember(4)]
        /// <summary>
        /// Gets or sets a value indicating whether the timer should accumulate time
        /// </summary>
        public bool Accumulate { get; set; }
        
        // [Name("Repeat")]
        // [ProtoMember(5)]
        /// <summary>
        /// Gets or sets a value indicating whether the timer should repeat
        /// </summary>
        public bool Repeat { get; set; }
        
        // [Name("CurrentDuration")]
        // [ProtoMember(6)]
        /// <summary>
        /// Gets or sets the current duration of the timer
        /// </summary>
        public float CurrentDuration { get; set; }
        
        // [Name("DefaultDuration")]
        // [ProtoMember(7)]
        /// <summary>
        /// Gets or sets the default duration of the timer
        /// </summary>
        public float DefaultDuration { get; set; }
    }
}