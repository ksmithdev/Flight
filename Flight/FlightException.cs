using System;
using System.Runtime.Serialization;

namespace Flight
{
    [Serializable]
    public class FlightException : Exception
    {
        public FlightException() : base()
        {
        }

        public FlightException(string message) : base(message)
        {
        }

        public FlightException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FlightException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}