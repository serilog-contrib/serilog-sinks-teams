using Newtonsoft.Json;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.MicrosoftTeams
{
    /// <summary>
    /// Implements <see cref="PeriodicBatchingSink"/> and provides means needed for sending Serilog log events to Microsoft Teams.
    /// </summary>
    public class MicrosoftTeamsSink : PeriodicBatchingSink
    {
        private static readonly HttpClient Client = new HttpClient();

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly MicrosoftTeamsSinkOptions _options;

        /// <summary>
        /// Initializes new instance of <see cref="MicrosoftTeamsSink"/>.
        /// </summary>
        /// <param name="options">Microsoft teams sink options object.</param>
        public MicrosoftTeamsSink(MicrosoftTeamsSinkOptions options)
            : base(options.BatchSizeLimit, options.Period)
        {
            _options = options;
        }

        /// <inheritdoc cref="PeriodicBatchingSink"/>
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                var message = CreateMessage(logEvent);
                var json = JsonConvert.SerializeObject(message, JsonSerializerSettings);
                var result = await Client.PostAsync(_options.WebHookUri, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(false);

                if (!result.IsSuccessStatusCode)
                {
                    throw new LoggingFailedException($"Received failed result {result.StatusCode} when posting events to Microsoft Teams");
                }
            }
        }

        /// <inheritdoc cref="PeriodicBatchingSink"/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Client.Dispose();
        }

        private MicrosoftTeamsMessageCard CreateMessage(LogEvent logEvent)
        {
            var renderedMessage = logEvent.RenderMessage(_options.FormatProvider);

            var request = new MicrosoftTeamsMessageCard
            {
                Title = _options.Title,
                Text = renderedMessage,
                Color = GetAttachmentColor(logEvent.Level),
                Sections = _options.RenderProperties
                    ? new[]
                    {
                        new MicrosoftTeamsMessageSection
                        {
                            Title = "Properties",
                            Facts = GetFacts(logEvent).ToArray()
                        }
                    }
                    : Enumerable.Empty<MicrosoftTeamsMessageSection>().ToArray()
            };

            return request;
        }

        private IEnumerable<MicrosoftTeamsMessageFact> GetFacts(LogEvent logEvent)
        {
            yield return new MicrosoftTeamsMessageFact
            {
                Name = "Level",
                Value = logEvent.Level.ToString()
            };
            yield return new MicrosoftTeamsMessageFact
            {
                Name = "MessageTemplate",
                Value = logEvent.MessageTemplate.Text
            };

            if (logEvent.Exception != null)
            {
                yield return new MicrosoftTeamsMessageFact { Name = "Exception", Value = logEvent.Exception.ToString() };
            }

            foreach (var property in logEvent.Properties)
            {
                yield return new MicrosoftTeamsMessageFact
                {
                    Name = property.Key,
                    Value = property.Value.ToString(null, _options.FormatProvider)
                };
            }
        }

        private static string GetAttachmentColor(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Information:
                    return "5bc0de";

                case LogEventLevel.Warning:
                    return "f0ad4e";

                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    return "d9534f";

                default:
                    return "777777";
            }
        }
    }
}