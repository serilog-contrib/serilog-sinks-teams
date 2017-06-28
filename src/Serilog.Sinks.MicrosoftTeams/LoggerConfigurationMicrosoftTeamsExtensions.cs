using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MicrosoftTeams;
using System;

namespace Serilog
{
    /// <summary>
    /// Provides extension methods on <see cref="LoggerSinkConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationMicrosoftTeamsExtensions
    {
        /// <summary>
        /// <see cref="LoggerSinkConfiguration"/> extension that provides configuration chaining.
        /// <example>
        ///     new LoggerConfiguration()
        ///         .MinimumLevel.Verbose()
        ///         .WriteTo.MicrosoftTeams("webHookUri")
        ///         .CreateLogger();
        /// </example>
        /// </summary>
        /// <param name="loggerSinkConfiguration">Instance of <see cref="LoggerSinkConfiguration"/> object.</param>
        /// <param name="webhookUrl">Slack team post URI.</param>
        /// <param name="batchSizeLimit">The time to wait between checking for event batches.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="title">Title that should be passed to the message.</param>
        /// <param name="formatProvider"><see cref="IFormatProvider"/> used to format the message.</param>
        /// <param name="restrictedToMinimumLevel"><see cref="LogEventLevel"/> value that specifies minimum logging level that will be allowed to be logged.</param>
        /// <returns>Instance of <see cref="LoggerConfiguration"/> object.</returns>
        public static LoggerConfiguration MicrosoftTeams(
            this LoggerSinkConfiguration loggerConfiguration,
            string webHookUri,
            string title = null,
            int? batchSizeLimit = null,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            var microsoftTeamsSinkOptions = new MicrosoftTeamsSinkOptions
            {
                WebHookUri = webHookUri,
            };

            if (!string.IsNullOrEmpty(title))
            {
                microsoftTeamsSinkOptions.Title = title;
            }

            if (batchSizeLimit.HasValue)
            {
                microsoftTeamsSinkOptions.BatchSizeLimit = batchSizeLimit.Value;
            }

            if (period.HasValue)
            {
                microsoftTeamsSinkOptions.Period = period.Value;
            }

            if (formatProvider != null)
            {
                microsoftTeamsSinkOptions.FormatProvider = formatProvider;
            }

            return loggerConfiguration.MicrosoftTeams(microsoftTeamsSinkOptions, restrictedToMinimumLevel);
        }

        /// <summary>
        /// <see cref="LoggerSinkConfiguration"/> extension that provides configuration chaining.
        /// </summary>
        /// <param name="loggerSinkConfiguration">Instance of <see cref="LoggerSinkConfiguration"/> object.</param>
        /// <param name="microsoftTeamsSinkOptions">The microsoft teams sink options object.</param>
        /// <param name="restrictedToMinimumLevel"><see cref="LogEventLevel"/> value that specifies minimum logging level that will be allowed to be logged.</param>
        /// <returns>Instance of <see cref="LoggerConfiguration"/> object.</returns>
        public static LoggerConfiguration  MicrosoftTeams(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            MicrosoftTeamsSinkOptions microsoftTeamsSinkOptions,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (loggerSinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerSinkConfiguration));
            }

            if (microsoftTeamsSinkOptions == null)
            {
                throw new ArgumentNullException(nameof(microsoftTeamsSinkOptions));
            }

            if (string.IsNullOrWhiteSpace(microsoftTeamsSinkOptions.WebHookUri))
            {
                throw new ArgumentNullException(nameof(microsoftTeamsSinkOptions.WebHookUri));
            }

            return loggerSinkConfiguration.Sink(new MicrosoftTeamsSink(microsoftTeamsSinkOptions), restrictedToMinimumLevel);
        }
    }
}