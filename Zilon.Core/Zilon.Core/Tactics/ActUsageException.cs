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
}