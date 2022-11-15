namespace Flight;

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

/// <summary>
/// Factory methods for generating localized exception messages.
/// </summary>
public static class FlightExceptionFactory
{
    private static ResourceManager ExceptionMessages { get; } = new ResourceManager("Flight.Resources.ExceptionMessages", Assembly.GetExecutingAssembly());

    /// <summary>
    /// Returns an <see cref="InvalidOperationException"/> with a localized error message.
    /// </summary>
    /// <param name="name">The name of the exception message to retrieve.</param>
    /// <returns>The localized <see cref="InvalidOperationException"/> exception.</returns>
    internal static InvalidOperationException InvalidOperation(string name) => new(ExceptionMessages.GetString(name, CultureInfo.CurrentCulture));

    /// <summary>
    /// Returns a <see cref="FlightException"/> with a localized error message.
    /// </summary>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <returns>The localized <see cref="FlightException"/> exception.</returns>
    internal static FlightException Unknown(Exception innerException) => new(ExceptionMessages.GetString("Unknown", CultureInfo.CurrentCulture), innerException);
}