namespace Flight.Logging
{
    using System;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// Defines a singleton logging provider.
    /// </summary>
    internal static class Log
    {
        private static ILogger logger = NullLogger.Instance;

        /// <summary>
        /// Writes a critical message to the log.
        /// </summary>
        /// <param name="crit">The message to log.</param>
        public static void Crit(string crit) => logger.LogCritical(crit);

        /// <summary>
        /// Writes a critical message along with exception stacktrace to the log.
        /// </summary>
        /// <param name="exception">The exception for stacktrace information.</param>
        /// <param name="crit">The message to log.</param>
        public static void Crit(Exception exception, string crit) => logger.LogCritical(exception, crit);

        /// <summary>
        /// Writes a debug message to the log.
        /// </summary>
        /// <param name="debug">The message to log.</param>
        public static void Debug(string debug) => logger.LogDebug(debug);

        /// <summary>
        /// Writes a debug message along with exception stacktrace to the log.
        /// </summary>
        /// <param name="exception">The exception for stacktrace information.</param>
        /// <param name="debug">The message to log.</param>
        public static void Debug(Exception exception, string debug) => logger.LogDebug(exception, debug);

        /// <summary>
        /// Writes an error message to the log.
        /// </summary>
        /// <param name="error">The message to log.</param>
        public static void Error(string error) => logger.LogError(error);

        /// <summary>
        /// Writes an error message along with exception stacktrace to the log.
        /// </summary>
        /// <param name="exception">The exception for stacktrace information.</param>
        /// <param name="error">The message to log.</param>
        public static void Error(Exception exception, string error) => logger.LogError(exception, error);

        /// <summary>
        /// Writes an information message to the log.
        /// </summary>
        /// <param name="info">The message to log.</param>
        public static void Info(string info) => logger.LogInformation(info);

        /// <summary>
        /// Writes an information message along with exception stacktrace to the log.
        /// </summary>
        /// <param name="exception">The exception for stacktrace information.</param>
        /// <param name="info">The message to log.</param>
        public static void Info(Exception exception, string info) => logger.LogInformation(exception, info);

        /// <summary>
        /// Sets the logger instance for the singleton.
        /// </summary>
        /// <param name="logger">The logger to use when writing log information.</param>
        public static void SetLogger(ILogger logger) => Log.logger = logger ?? NullLogger.Instance;

        /// <summary>
        /// Writes a trace message to the log.
        /// </summary>
        /// <param name="trace">The message to log.</param>
        public static void Trace(string trace) => logger.LogTrace(trace);

        /// <summary>
        /// Writes a trace message along with exception stacktrace to the log.
        /// </summary>
        /// <param name="exception">The exception for stacktrace information.</param>
        /// <param name="trace">The message to log.</param>
        public static void Trace(Exception exception, string trace) => logger.LogTrace(exception, trace);

        /// <summary>
        /// Writes a warning message to the log.
        /// </summary>
        /// <param name="warn">The message to log.</param>
        public static void Warn(string warn) => logger.LogWarning(warn);

        /// <summary>
        /// Writes a warning message along with exception stacktrace to the log.
        /// </summary>
        /// <param name="exception">The exception for stacktrace information.</param>
        /// <param name="warn">The message to log.</param>
        public static void Warn(Exception exception, string warn) => logger.LogWarning(exception, warn);
    }
}