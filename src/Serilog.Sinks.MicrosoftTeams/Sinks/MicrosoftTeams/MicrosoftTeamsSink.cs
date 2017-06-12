using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.MicrosoftTeams
{
    public class MicrosoftTeamsSink : ILogEventSink
    {
        private readonly string _webHookUri;
        private readonly IFormatProvider _formatProvider;
        private readonly string _title;

        private readonly HttpClient _client = new HttpClient();

        public MicrosoftTeamsSink(string webHookUri, IFormatProvider formatProvider, string title)
        {
            _webHookUri = webHookUri;
            _formatProvider = formatProvider;
            _title = title;
        }

        public void Emit(LogEvent logEvent)
        {
            var request = JsonConvert.SerializeObject(CreateMessageRequest(logEvent));
            Task.Run(() => _client.PostAsync(_webHookUri,
                new StringContent(request, Encoding.UTF8, "application/json"))).Wait();
        }

        private MicrosoftTeamsMessageCard CreateMessageRequest(LogEvent logEvent)
        {
            var renderedMessage = logEvent.RenderMessage(_formatProvider);

            var request = new MicrosoftTeamsMessageCard
            {
                Title = _title,
                Text = renderedMessage,
                Color = GetAttachmentColor(logEvent.Level),
                Sections = new[]
                {
                    new MicrosoftTeamsMessageSection
                    {
                        Title = "Properties",
                        Facts = GetFacts(logEvent).ToArray()
                    }
                }
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
                    Value = property.Value.ToString(null, _formatProvider)
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