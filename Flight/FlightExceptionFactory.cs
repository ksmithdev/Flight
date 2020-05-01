using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Flight
{
    public static class FlightExceptionFactory
    {
        private static ResourceManager ExceptionMessages { get; } = new ResourceManager("Flight.Resources.ExceptionMessages", Assembly.GetExecutingAssembly());

        internal static Exception InvalidOperation(string name) => new InvalidOperationException(ExceptionMessages.GetString(name, CultureInfo.CurrentCulture));

        internal static FlightException Unknown(Exception innerException) => new FlightException(ExceptionMessages.GetString("Unknown", CultureInfo.CurrentCulture), innerException);
    }
}