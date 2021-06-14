using System;
using System.Runtime.Serialization;

namespace Zilon.Core.Tactics.Behaviour
{
    [Serializable]
    public class ActorTaskSourceException : Exception
    {
        public ActorTaskSourceException() { }
        public ActorTaskSourceException(string message) : base(message) { }
        public ActorTaskSourceException(string message, Exception inner) : base(message, inner) { }

        protected ActorTaskSourceException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}