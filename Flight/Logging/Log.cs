namespace Flight.Logging
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using System;

    /// <summary>Defines a singleton logging provider</summary>
    public static class Log
    {
        private static ILogger Logger = NullLogger.Instance;

        /// <summary>Writes a critical message to the log</summary>
        /// <param name="crit">The message to log</param>
        public static void Crit(string crit) => Logger.LogCritical(crit);

        /// <summary>Writes a critical message along with exception stacktrace to the log</summary>
        /// <param name="exception">The exception for stacktrace information</param>
        /// <param name="crit">The message to log</param>
        public static void Crit(Exception exception, string crit) => Logger.LogCritical(exception, crit);

        /// <summary>Writes a debug message to the log</summary>
        /// <param name="debug">The message to log</param>
        public static void Debug(string debug) => Logger.LogDebug(debug);

        /// <summary>Writes a debug message along with exception stacktrace to the log</summary>
        /// <param name="exception">The exception for stacktrace information</param>
        /// <param name="debug">The message to log</param>
        public static void Debug(Exception exception, string debug) => Logger.LogDebug(exception, debug);

        /// <summary>Writes an error message to the log</summary>
        /// <param name="error">The message to log</param>
        public static void Error(string error) => Logger.LogError(error);

        /// <summary>Writes an error message along with exception stacktrace to the log</summary>
        /// <param name="exception">The exception for stacktrace information</param>
        /// <param name="error">The message to log</param>
        public static void Error(Exception exception, string error) => Logger.LogError(exception, error);

        /// <summary>Writes an information message to the log</summary>
        /// <param name="warn">The message to log</param>
        public static void Info(string info) => Logger.LogInformation(info);

        /// <summary>Writes an information message along with exception stacktrace to the log</summary>
        /// <param name="exception">The exception for stacktrace information</param>
        /// <param name="info">The message to log</param>
        public static void Info(Exception exception, string info) => Logger.LogInformation(exception, info);

        /// <summary>Sets the logger instance for the singleton</summary>
        /// <param name="logger">The logger to use when writing log information</param>
        public static void SetLogger(ILogger logger) => Logger = logger ?? NullLogger.Instance;

        /// <summary>Writes a trace message to the log</summary>
        /// <param name="warn">The message to log</param>
        public static void Trace(string trace) => Logger.LogTrace(trace);

        /// <summary>Writes a trace message along with exception stacktrace to the log</summary>
        /// <param name="exception">The exception for stacktrace information</param>
        /// <param name="trace">The message to log</param>
        public static void Trace(Exception exception, string trace) => Logger.LogTrace(exception, trace);

        /// <summary>Writes a warning message to the log</summary>
        /// <param name="warn">The message to log</param>
        public static void Warn(string warn) => Logger.LogWarning(warn);

        /// <summary>Writes a warning message along with exception stacktrace to the log</summary>
        /// <param name="exception">The exception for stacktrace information</param>
        /// <param name="warn">The message to log</param>
        public static void Warn(Exception exception, string warn) => Logger.LogWarning(exception, warn);
    }
}