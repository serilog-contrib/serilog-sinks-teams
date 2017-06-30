using System;

namespace Serilog.Sinks.MicrosoftTeams
{
    /// <summary>
    /// Container for all Microsoft Teams sink configuration.
    /// </summary>
    public class MicrosoftTeamsSinkOptions
    {
        private static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(5);
        private const int DefaultBatchSizeLimit = 50;

        public MicrosoftTeamsSinkOptions(string webHookUri, string title, int? batchSizeLimit = null, TimeSpan? period = null, IFormatProvider formatProvider = null)
        {
            if (webHookUri == null)
            {
                throw new ArgumentNullException(nameof(webHookUri));
            }

            if (string.IsNullOrEmpty(webHookUri))
            {
                throw new ArgumentException(nameof(webHookUri));
            }

            WebHookUri = webHookUri;
            Title = title;
            BatchSizeLimit = batchSizeLimit ?? DefaultBatchSizeLimit;
            Period = period ?? DefaultPeriod;
            FormatProvider = formatProvider;
        }

        /// <summary>
        /// Required: The incoming webhook URI from your microsoft teams integrations page.
        /// </summary>
        public string WebHookUri { get; }
        
        /// <summary>
        /// Optional: Format provider used for formatting the message.
        /// </summary>
        public IFormatProvider FormatProvider { get; }

        /// <summary>
        /// Optional: Title of message.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Optional: How many messages to send to microsoft teams at once.
        /// </summary>
        public int BatchSizeLimit { get; }

        /// <summary>
        /// Optional: The maximum period between message batches.
        /// </summary>
        public TimeSpan Period { get; }
    }
}