using System;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{

    [Serializable]
    public class ActorTaskExecutionException : Exception
    {
        public ActorTaskExecutionException(IActorTaskSource actorTaskSource)
        {
            ActorTaskSource = actorTaskSource;
        }

        public ActorTaskExecutionException(string message, IActorTaskSource actorTaskSource) : base(message)
        {
            ActorTaskSource = actorTaskSource;
        }

        public ActorTaskExecutionException(string message, IActorTaskSource actorTaskSource, Exception inner) : base(message, inner)
        {
            ActorTaskSource = actorTaskSource;
        }

        protected ActorTaskExecutionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public IActorTaskSource ActorTaskSource { get; }
    }
}