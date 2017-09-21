using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;
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
        private readonly string[] _exceptionsToGroupByMessage;

        /// <summary>
        /// Construct a sink that saves logs to the specified storage account.
        /// </summary>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="dsn">The DSN as found on the sentry.io website.</param>
        public SentryIOSink(IFormatProvider formatProvider, string dsn, string release = "", string exceptionsToGroupByMessage = "")
        {
            _formatProvider = formatProvider;
            _logger = new RavenClient(dsn) { Release = release };
            _exceptionsToGroupByMessage = exceptionsToGroupByMessage.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
            var sentryEvent = new SentryEvent(logEvent.Exception)
            {
                Level = LevelToSeverity(logEvent),
                Message = GetSentryMessage(logEvent, logEvent.Properties, _formatProvider),
                Tags = PropertiesToData(logEvent).ToDictionary(k => k.Key, v => v.Value?.ToString()),
                Extra = PropertiesToDataWithException(logEvent)
            };

            var fingerprints = GetFingerprints(logEvent);

            foreach (var fingerprint in fingerprints)
            {
                sentryEvent.Fingerprint.Add(fingerprint);
            }
            
            _logger.Capture(sentryEvent);
        }

        private IList<string> GetFingerprints(LogEvent logEvent)
        {
            var list = new List<string>();

            var exception = logEvent.Exception;
            if (exception != null)
            {
                var exceptionType = exception.GetType().Name;

                if (_exceptionsToGroupByMessage.Contains(exceptionType))
                {
                    list.Add(exception.Message);
                }
            }

            return list;
        }

        private static SentryMessage GetSentryMessage(LogEvent logEvent, IReadOnlyDictionary<string, LogEventPropertyValue> properties, IFormatProvider formatProvider)
        {
            var textWriter = new StringWriter(formatProvider);

            var parameters = new List<object>();
            foreach (MessageTemplateToken token in logEvent.MessageTemplate.Tokens)
            {
                var propertyToken = token as PropertyToken;
                if (propertyToken != null)
                {
                    var parameter = properties.ContainsKey(propertyToken.PropertyName) ? properties[propertyToken.PropertyName].ToString() : "";
                    
                    textWriter.Write("{" + parameters.Count + "}");
                    parameters.Add(parameter);

                    continue;
                }
                
                token.Render(properties, textWriter, formatProvider);
            }
            
            return new SentryMessage(textWriter.ToString(), parameters.ToArray());
        }

        static IDictionary<string, object> PropertiesToData(LogEvent logEvent)
        {
            var data = new Dictionary<string, object>();

            if (logEvent.Exception != null)
            {
                foreach (var key in logEvent.Exception.Data.Keys)
                {
                    data[key.ToString()] = logEvent.Exception.Data[key];
                }
            }

            foreach (var property in logEvent.Properties)
            {
                data[property.Key] = property.Value.ToString();
            }

            return data;
        }

        static IDictionary<string, object> PropertiesToDataWithException(LogEvent logEvent)
        {
            var data = PropertiesToData(logEvent);

            if (logEvent.Exception != null)
            {
                data["exception"] = logEvent.Exception;
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
