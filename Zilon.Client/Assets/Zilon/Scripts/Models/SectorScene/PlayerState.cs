using Zilon.Core.Client;
using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.Models.SectorScene
{
    internal class PlayerState : IPlayerState
    {
        public IActorViewModel ActiveActor { get; set; }

        public IMapNodeViewModel SelectedNode { get; set; }

        public HumanActorTaskSource TaskSource { get; set; }

        public IActorViewModel SelectedActor { get; set; }
        
        public IContainerViewModel SelectedContainer { get; set; }
    }
}