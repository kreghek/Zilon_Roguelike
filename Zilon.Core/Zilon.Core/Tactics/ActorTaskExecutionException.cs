using System;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    [Serializable]
    public class ActorTaskExecutionException : Exception
    {
        public ActorTaskExecutionException(IActorTaskSource<ISectorTaskSourceContext> actorTaskSource)
        {
            ActorTaskSource = actorTaskSource;
        }

        public ActorTaskExecutionException(string message, IActorTaskSource<ISectorTaskSourceContext> actorTaskSource) :
            base(message)
        {
            ActorTaskSource = actorTaskSource;
        }

        public ActorTaskExecutionException(
            string message,
            IActorTaskSource<ISectorTaskSourceContext> actorTaskSource,
            Exception inner) : base(message, inner)
        {
            ActorTaskSource = actorTaskSource;
        }

        public ActorTaskExecutionException()
        {
        }

        public ActorTaskExecutionException(string message) : base(message)
        {
        }

        public ActorTaskExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ActorTaskExecutionException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public IActorTaskSource<ISectorTaskSourceContext> ActorTaskSource { get; }
    }
}