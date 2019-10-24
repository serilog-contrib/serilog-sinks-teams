# Serilog.Sinks.MicrosoftTeams

----------

[![NuGet Version](https://img.shields.io/nuget/v/Serilog.Sinks.MicrosoftTeams.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.MicrosoftTeams/)

A Serilog event sink that writes to [Microsoft Teams](https://teams.microsoft.com).

**Package** - [Serilog.Sinks.MicrosoftTeams](https://www.nuget.org/packages/Serilog.Sinks.MicrosoftTeams/)
| **Platforms** - .NETStandard 2.0

You need to add an "Incoming Webhook" connector to your Teams channel and get it's URL. `title` is optional but can help your distinguish logs coming from different sources.

```csharp
var log = new LoggerConfiguration()
    .WriteTo.MicrosoftTeams(webHookUri, title: title)
    .CreateLogger();
```

### JSON configuration

It is possible to configure the sink using [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) by specifying the table name and connection string in `appsettings.json`:

```json
"Serilog": {
  "WriteTo": [
    {"Name": "MicrosoftTeams", "Args": {"webHookUri": "", "title": ""}}
  ]
}
```

JSON configuration must be enabled using `ReadFrom.Configuration()`; see the [documentation of the JSON configuration package](https://github.com/serilog/serilog-settings-configuration) for details.