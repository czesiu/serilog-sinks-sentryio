# Serilog.Sinks.SentryIO [![Build status](https://ci.appveyor.com/api/projects/status/hh9gymy0n6tne46j?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-file) [![Travis build](https://travis-ci.org/serilog/serilog-sinks-file.svg)](https://travis-ci.org/serilog/serilog-sinks-file) [![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.File.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.File/) [![Documentation](https://img.shields.io/badge/docs-wiki-yellow.svg)](https://github.com/serilog/serilog/wiki) [![Join the chat at https://gitter.im/serilog/serilog](https://img.shields.io/gitter/room/serilog/serilog.svg)](https://gitter.im/serilog/serilog)

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