namespace Flight.Logging
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using System;

    public static class Log
    {
        private static ILogger Logger = NullLogger.Instance;

        public static void Crit(string message) => Logger.LogCritical(message);

        public static void Crit(Exception exception, string message) => Logger.LogCritical(exception, message);

        public static void Debug(string message) => Logger.LogDebug(message);

        public static void Debug(Exception exception, string message) => Logger.LogDebug(exception, message);

        public static void Error(string message) => Logger.LogError(message);

        public static void Error(Exception exception, string message) => Logger.LogError(exception, message);

        public static void Info(string message) => Logger.LogInformation(message);

        public static void Info(Exception exception, string message) => Logger.LogInformation(exception, message);

        public static void SetLogger(ILogger logger) => Logger = logger ?? NullLogger.Instance;

        public static void Trace(string message) => Logger.LogTrace(message);

        public static void Trace(Exception exception, string message) => Logger.LogTrace(exception, message);

        public static void Warn(string message) => Logger.LogWarning(message);

        public static void Warn(Exception exception, string message) => Logger.LogWarning(exception, message);
    }
}