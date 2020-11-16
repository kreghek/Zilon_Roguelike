using System;

namespace Zilon.Core.Tactics
{
    [Serializable]
    public class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException() { }
        public HandlerNotFoundException(string message) : base(message) { }
        public HandlerNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected HandlerNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}