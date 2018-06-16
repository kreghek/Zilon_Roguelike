using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Models.SectorScene;

namespace Assets.Zilon.Scripts.Models.Commands
{
    /// <summary>
    /// Команда на перемещение взвода в указанный узел карты.
    /// </summary>
    class AttackCommand : ActorCommandBase
    {

        public AttackCommand(ISectorManager sectorManager,
            IPlayerState playerState) : 
            base(sectorManager, playerState)
        {
            
        }

        public override bool CanExecute()
        {
            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            var sector = _sectorManager.CurrentSector;
            var selectedActorVM = _playerState.SelectedActor;

            var targetActor = selectedActorVM.Actor;
            _playerState.TaskSource.IntentAttack(targetActor);
            sector.Update();
        }
    }
}