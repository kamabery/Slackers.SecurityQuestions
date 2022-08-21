namespace Slackers.Logging;
 
    /// <summary>
    /// Configuration Options for Logging
    /// </summary>
    public class LoggingOptions
    {
        /// <summary>
        /// Gets or sets File Location if writing to file
        /// </summary>
        public string FileLocation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets File Name
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Interval file roles
        /// </summary>
        public int RollingInterval { get; set; } = 1;

    }

