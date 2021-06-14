using System;

namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class ActorTaskContext : IActorTaskContext
    {
        public ActorTaskContext(ISector sector)
        {
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
        }

        public ISector Sector { get; }
    }
}