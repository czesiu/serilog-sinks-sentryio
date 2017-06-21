using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;
using SharpRaven;
using SharpRaven.Data;

namespace Serilog.Sinks.SentryIO
{
    /// <summary>
    /// Writes log events to the Sentry.IO service.
    /// </summary>
    public class SentryIOSink : ILogEventSink
    {
        readonly IFormatProvider _formatProvider;
        readonly IRavenClient _logger;

        /// <summary>
        /// Construct a sink that saves logs to the specified storage account.
        /// </summary>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="dsn">The DSN as found on the sentry.io website.</param>
        public SentryIOSink(IFormatProvider formatProvider, string dsn)
        {
            _formatProvider = formatProvider;
            _logger = new RavenClient(dsn);
        }

        /// <summary>
        /// Construct a sink that saves logs to the specified logger. The purpose of this
        /// constructor is to re-use an existing IRavenClient from SharpRaven.
        /// </summary>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="logger">The logger to use.</param>
        public SentryIOSink(IFormatProvider formatProvider, IRavenClient logger)
        {
            _formatProvider = formatProvider;
            _logger = logger;
        }

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            var sentryEvent = new SentryEvent(logEvent.Exception)
            {
                Level = LevelToSeverity(logEvent),
                Message = new SentryMessage(message),
                Tags = PropertiesToData(logEvent),
                Extra = PropertiesToData(logEvent)
            };
            
            _logger.Capture(sentryEvent);
        }


        static IDictionary<string, string> PropertiesToData(LogEvent logEvent)
        {
            var data = new Dictionary<string, string>();

            if (logEvent.Exception != null)
            {
                foreach (var key in logEvent.Exception.Data.Keys)
                {
                    data[key.ToString()] = logEvent.Exception.Data[key].ToString();
                }
            }

            foreach (var property in logEvent.Properties)
            {
                data[property.Key] = property.Value.ToString();
            }

            return data;
        }

        static ErrorLevel LevelToSeverity(LogEvent logEvent)
        {
            switch (logEvent.Level)
            {
                case LogEventLevel.Fatal:
                    return ErrorLevel.Fatal;
                case LogEventLevel.Error:
                    return ErrorLevel.Error;
                case LogEventLevel.Warning:
                    return ErrorLevel.Warning;
                case LogEventLevel.Information:
                    return ErrorLevel.Info;
                default:
                    return ErrorLevel.Debug;
            }
        }
    }
}
