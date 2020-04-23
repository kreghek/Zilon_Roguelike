using System;
using System.Runtime.Serialization;

namespace Zilon.Core.Tactics
{
    [Serializable]
    public class ActUsageException : Exception
    {
        public ActUsageException() { }
        public ActUsageException(string message) : base(message) { }
        public ActUsageException(string message, Exception inner) : base(message, inner) { }
        protected ActUsageException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }

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

    [Serializable]
    public class UsageThroughtWallException : ActUsageException
    {
        public UsageThroughtWallException(string message) : base(message)
        {
        }

        public UsageThroughtWallException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UsageThroughtWallException()
        {
        }

        protected UsageThroughtWallException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
