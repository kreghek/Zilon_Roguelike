using System;

namespace Zilon.Core.Tactics.Behaviour
{
    [Serializable]
    public class ActorTaskSourceException : Exception
    {
        public ActorTaskSourceException() { }
        public ActorTaskSourceException(string message) : base(message) { }
        public ActorTaskSourceException(string message, Exception inner) : base(message, inner) { }

        protected ActorTaskSourceException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}