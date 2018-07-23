using Assets.Zilon.Scripts.Models.SectorScene;

namespace Assets.Zilon.Scripts.Models.Commands
{
    class NextTurnCommand: ActorCommandBase
    {
        
        public override bool CanExecute()
        {
            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            var sector = _sectorManager.CurrentSector;
            sector.Update();
        }

        public NextTurnCommand(ISectorManager sectorManager, 
            IPlayerState playerState) :
            base(sectorManager, playerState)
        {
        }
    }
}