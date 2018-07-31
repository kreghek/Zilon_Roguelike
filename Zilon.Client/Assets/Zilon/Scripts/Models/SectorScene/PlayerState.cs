using Zilon.Core.Client;
using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.Models.SectorScene
{
    internal class PlayerState : IPlayerState
    {
        public IActorViewModel ActiveActor { get; set; }

        public ISelectableViewModel HoverViewModel { get; set; }

        public HumanActorTaskSource TaskSource { get; set; }
    }
}