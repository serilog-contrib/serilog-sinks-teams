using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MicrosoftTeams;
using System;

namespace Serilog
{
    public static class LoggerConfigurationMicrosoftTeamsExtensions
    {
        public static LoggerConfiguration MicrosoftTeams(
            this LoggerSinkConfiguration loggerConfiguration,
            string webHookUri,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string title = null,
            IFormatProvider formatProvider = null)
        {
            ILogEventSink sink;
            try
            {
                sink = new MicrosoftTeamsSink(webHookUri, formatProvider, title);
            }
            catch (Exception e)
            {
                Debugging.SelfLog.WriteLine("Error configuring MicrosoftTeams: {0}", e);
                sink = new LoggerConfiguration().CreateLogger();
            }

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}