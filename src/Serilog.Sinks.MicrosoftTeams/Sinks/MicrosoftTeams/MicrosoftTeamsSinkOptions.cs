using System;

namespace Serilog.Sinks.MicrosoftTeams
{
    /// <summary>
    /// Container for all Microsoft Teams sink configuration.
    /// </summary>
    public class MicrosoftTeamsSinkOptions
    {
        /// <summary>
        /// Required: The incoming webhook URI from your microsoft teams integrations page.
        /// </summary>
        public string WebHookUri { get; set; }
        
        public IFormatProvider FormatProvider { get; set; }
        public string Title { get; set; }

        /// <summary>
        /// Optional: How many messages to send to Slack at once. Defaults to 50.
        /// </summary>
        public int BatchSizeLimit { get; set; } = 50;

        /// <summary>
        /// Optional: The maximum period between message batches. Defaults to 5 seconds.
        /// </summary>
        public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(5);
    }
}