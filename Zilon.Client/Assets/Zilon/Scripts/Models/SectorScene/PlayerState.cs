using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.Models.SectorScene
{
    internal class PlayerState : IPlayerState
    {
        public ActorVM ActiveActor { get; set; }

        public MapNodeVM SelectedNode { get; set; }

        public HumanActorTaskSource TaskSource { get; set; }

        public ActorVM SelectedActor { get; set; }
    }
}