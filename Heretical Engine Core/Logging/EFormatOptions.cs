using System;

namespace HereticalSolutions.Logging
{
    /// <summary>
    /// Specifies the formatting options for text logging.
    /// </summary>
    [Flags]
    public enum EFormatOptions
    {
        /// <summary>
        /// No formatting option.
        /// </summary>
        NONE = 0,
        
        /// <summary>
        /// Bold formatting option.
        /// </summary>
        TEXT_BOLD = 1 << 0, //<b>
        
        /// <summary>
        /// Italics formatting option.
        /// </summary>
        TEXT_ITALICS = 1 << 1, //<i>
        
        /// <summary>
        /// Strikethrough formatting option.
        /// </summary>
        TEXT_STRIKETHROUGH = 1 << 2, //<s>
        
        /// <summary>
        /// Underline formatting option.
        /// </summary>
        TEXT_UNDERLINE = 1 << 3, //<u>
        
        /// <summary>
        /// All caps formatting option.
        /// </summary>
        TEXT_CAPS = 1 << 4, //<allcaps>
        
        /// <summary>
        /// Small font size formatting option.
        /// </summary>
        TEXT_SIZE_SMALL = 1 << 5,
        
        /// <summary>
        /// Big font size formatting option.
        /// </summary>
        TEXT_SIZE_BIG = 1 << 6,
        
        /// <summary>
        /// Good color formatting option.
        /// </summary>
        COLOR_GOOD = 1 << 7,
        
        /// <summary>
        /// Bad color formatting option.
        /// </summary>
        COLOR_BAD = 1 << 8,
        
        /// <summary>
        /// Dependency color formatting option.
        /// </summary>
        COLOR_DEPENDENCY = 1 << 9,
        
        /// <summary>
        /// Event color formatting option.
        /// </summary>
        COLOR_EVENT = 1 << 10,
        
        /// <summary>
        /// Initialization color formatting option.
        /// </summary>
        COLOR_INIT = 1 << 11,
        
        /// <summary>
        /// Deinitialization color formatting option.
        /// </summary>
        COLOR_DEINIT = 1 << 12
    }
}