using Newtonsoft.Json;
using System.Collections.Generic;

namespace Serilog.Sinks.MicrosoftTeams
{
    internal class MicrosoftTeamsMessageSection
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("facts")]
        public IList<MicrosoftTeamsMessageFact> Facts { get; set; }
    }
}