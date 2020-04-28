using System;
using System.Runtime.Serialization;

namespace Zilon.Core.Tactics
{
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
