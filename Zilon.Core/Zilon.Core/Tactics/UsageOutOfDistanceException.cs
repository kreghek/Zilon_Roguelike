using System;
using System.Runtime.Serialization;

namespace Zilon.Core.Tactics
{
    [Serializable]
    public class UsageOutOfDistanceException : ActUsageException
    {
        public UsageOutOfDistanceException(string message) : base(message)
        {
        }

        public UsageOutOfDistanceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UsageOutOfDistanceException()
        {
        }

        protected UsageOutOfDistanceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}