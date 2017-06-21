# Serilog.Sinks.SentryIO [![Build status](https://ci.appveyor.com/api/projects/status/fbvwkg6cu66jmiq7?svg=true)](https://ci.appveyor.com/project/czesiu/serilog-sinks-sentryio) [![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.SentryIO.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.SentryIO/)

Writes [Serilog](https://serilog.net) events to a [Sentry.io](https://sentry.io) service.

```csharp
var log = new LoggerConfiguration()
    .WriteTo.SentryIO("{dsn-from-sentry-io-website}")
    .CreateLogger();
```

### `<appSettings>` configuration

The sink can be configured in XML [app-settings format](https://github.com/serilog/serilog/wiki/AppSettings) if the _Serilog.Settings.AppSettings_ package is in use:

```xml
<add key="serilog:using:SentryIO" value="Serilog.Sinks.SentryIO" />
<add key="serilog:write-to:SentryIO.dsn" value="{dsn-from-sentry-io-website}" />
```