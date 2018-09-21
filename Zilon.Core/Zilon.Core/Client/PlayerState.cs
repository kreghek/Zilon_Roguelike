using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Client
{
    public class PlayerState : IPlayerState
    {
        [ExcludeFromCodeCoverage]
        public IActorViewModel ActiveActor { get; set; }

        [ExcludeFromCodeCoverage]
        public ISelectableViewModel HoverViewModel { get; set; }

        [ExcludeFromCodeCoverage]
        public IHumanActorTaskSource TaskSource { get; set; }
    }
}
