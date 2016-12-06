using System;
using System.Runtime.Serialization;

namespace RssReader
{
    [Serializable]
    internal class BadXmlException : Exception
    {
        public BadXmlException()
        {
        }

        public BadXmlException(string message) : base(message)
        {
        }

        public BadXmlException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadXmlException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}