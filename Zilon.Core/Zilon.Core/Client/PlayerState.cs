using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Client
{
    public class PlayerState : IPlayerState
    {
        public IActorViewModel ActiveActor { get; set; }

        public ISelectableViewModel HoverViewModel { get; set; }

        public IHumanActorTaskSource TaskSource { get; set; }

        public ISelectableViewModel SelectedViewModel { get; set; }
    }
}
