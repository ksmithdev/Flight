namespace Flight;

using System;
using System.Runtime.Serialization;

/// <summary>
/// Represents errors that occur in the Flight framework.
/// </summary>
[Serializable]
public class FlightException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlightException"/> class.
    /// </summary>
    public FlightException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlightException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public FlightException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlightException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public FlightException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlightException"/> class.
    /// </summary>
    /// <param name="serializationInfo">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
    /// <param name="streamingContext">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
    protected FlightException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
    }
}