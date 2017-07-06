using System;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.SentryIO;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.SentryIO() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class SentryIOConfigurationExtensions
    {
        /// <summary>
        /// Adds a sink that writes log events to the sentry.io webservice. 
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="dsn">The DSN as found on the sentry.io website.</param>
        /// <param name="release">Version of application release.</param>
        /// <param name="exceptionsToGroupByMessage">List of exception types which should be grouped by message.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink. Set to Warning by default.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration SentryIO(
            this LoggerSinkConfiguration loggerConfiguration,
            string dsn,
            string release = "",
            string exceptionsToGroupByMessage = "",
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Warning,
            IFormatProvider formatProvider = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));

            return loggerConfiguration.Sink(
                new SentryIOSink(formatProvider, dsn, release, exceptionsToGroupByMessage),
                restrictedToMinimumLevel);
        }
    }
}
