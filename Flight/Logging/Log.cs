namespace Flight.Logging
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using System;

    public static class Log
    {
        private static ILogger Logger = NullLogger.Instance;

        public static void Crit(string crit) => Logger.LogCritical(crit);

        public static void Crit(Exception exception, string crit) => Logger.LogCritical(exception, crit);

        public static void Debug(string debug) => Logger.LogDebug(debug);

        public static void Debug(Exception exception, string debug) => Logger.LogDebug(exception, debug);

        public static void Error(string error) => Logger.LogError(error);

        public static void Error(Exception exception, string error) => Logger.LogError(exception, error);

        public static void Info(string info) => Logger.LogInformation(info);

        public static void Info(Exception exception, string info) => Logger.LogInformation(exception, info);

        public static void SetLogger(ILogger logger) => Logger = logger ?? NullLogger.Instance;

        public static void Trace(string trace) => Logger.LogTrace(trace);

        public static void Trace(Exception exception, string trace) => Logger.LogTrace(exception, trace);

        public static void Warn(string warn) => Logger.LogWarning(warn);

        public static void Warn(Exception exception, string warn) => Logger.LogWarning(exception, warn);
    }
}