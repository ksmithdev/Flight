namespace Flight
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class FlightException : Exception
    {
        public FlightException()
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