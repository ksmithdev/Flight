using System;

namespace Flight
{
    [Serializable]
    public class FlightConcurrencyException : FlightException
    {
        public FlightConcurrencyException()
        {
        }

        public FlightConcurrencyException(string message) : base(message)
        {
        }

        public FlightConcurrencyException(string message, Exception inner) : base(message, inner)
        {
        }

        protected FlightConcurrencyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}