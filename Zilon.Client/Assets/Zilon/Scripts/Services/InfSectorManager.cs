using System.Threading.Tasks;

using Zilon.Core.Players;
using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Services
{
    class InfSectorManager : ISectorManager
    {
        private readonly HumanPlayer _humanPlayer;

        public InfSectorManager(
            HumanPlayer humanPlayer)
        {
            _humanPlayer = humanPlayer;
        }

        public ISector CurrentSector { get; private set; }

        public async Task CreateSectorAsync()
        {
            CurrentSector = _humanPlayer.SectorNode.Sector;
        }
    }
}
