using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Players;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics
{
    public class InfiniteSectorManager : ISectorManager
    {
        private const string LOCATION_SID = "infinity-dungeon";

        private readonly ISectorGenerator _generator;
        private readonly HumanPlayer _humanPlayer;
        private readonly ISchemeService _schemeService;

        public InfiniteSectorManager(ISectorGenerator generator,
            HumanPlayer humanPlayer,
            ISchemeService schemeService)
        {
            _generator = generator;
            _humanPlayer = humanPlayer;
            _schemeService = schemeService;
        }

        public ISector CurrentSector { get; private set; }

        public async Task CreateSectorAsync()
        {
            var scheme = _schemeService.GetScheme<ILocationScheme>(LOCATION_SID);

            var sectorLevelScheme = scheme.SectorLevels[0];

            CurrentSector = await _generator.GenerateDungeonAsync(sectorLevelScheme);
            CurrentSector.Scheme = scheme;
        }
    }
}
