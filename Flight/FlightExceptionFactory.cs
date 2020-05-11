using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Flight
{
    /// <summary>
    /// Factory methods for generating localized exception messages.
    /// </summary>
    public static class FlightExceptionFactory
    {
        private static ResourceManager ExceptionMessages { get; } = new ResourceManager("Flight.Resources.ExceptionMessages", Assembly.GetExecutingAssembly());

        /// <summary>
        /// Returns an <see cref="InvalidOperationException"/> with a localized error message.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static Exception InvalidOperation(string name) => new InvalidOperationException(ExceptionMessages.GetString(name, CultureInfo.CurrentCulture));

        /// <summary>
        /// Returns a <see cref="FlightException"/> with a localized error message.
        /// </summary>
        /// <param name="innerException"></param>
        /// <returns></returns>
        internal static FlightException Unknown(Exception innerException) => new FlightException(ExceptionMessages.GetString("Unknown", CultureInfo.CurrentCulture), innerException);
    }
}