using System.ComponentModel;

namespace RemoteKeycard
{
    public class Config
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;
        
        /// <summary>
        /// Whether this plugin works on SCP lockers.
        /// </summary>
        [Description("Whether this plugin works on SCP lockers.")]
        public bool AffectScpLockers { get; set; } = true;

        /// <summary>
        /// Whether this plugin works on doors.
        /// </summary>
        [Description("Whether this plugin works on doors.")]
        public bool AffectDoors { get; set; } = true;
        
        /// <summary>
        /// Whether this plugin works on doors.
        /// </summary>
        [Description("Whether this plugin works on generators.")]
        public bool AffectGenerators { get; set; } = true;

    }
}