using Zilon.Core.Client;

namespace Zilon.Core.Commands
{
    //TODO Добавить тесты
    public class NextTurnCommand: ActorCommandBase
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