using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Services
{
    class InfSectorManager : ISectorManager
    {
        private readonly HumanPlayer _humanPlayer;
        private readonly ISectorGenerator _generator;
        private readonly ISchemeService _schemeService;

        private readonly string[] _availableSectorSids = new[] {
            "rat-hole",
            "rat-kingdom",
            "demon-dungeon",
            "demon-lair",
            "elder-place",
            "genomass-cave",
            "crypt"
        };

        public InfSectorManager(
            HumanPlayer humanPlayer,
            ISectorGenerator generator,
            ISchemeService schemeService)
        {
            _humanPlayer = humanPlayer;
            _generator = generator;
            _schemeService = schemeService;
        }

        public ISector CurrentSector { get; private set; }

        public async Task CreateSectorAsync()
        {
            var currentSchemeSid = _humanPlayer.SectorSid;
            if (currentSchemeSid == null)
            {
                currentSchemeSid = "intro";
            }
            else if (currentSchemeSid == "globe")
            {
                currentSchemeSid = _availableSectorSids[UnityEngine.Random.Range(0, _availableSectorSids.Length)];
            }

            var scheme = _schemeService.GetScheme<ILocationScheme>(currentSchemeSid);

            ISectorSubScheme sectorLevelScheme;
            var currentSectorLevelSid = _humanPlayer.SectorLevelSid;
            if (currentSectorLevelSid != null)
            {
                sectorLevelScheme = scheme.SectorLevels.Single(x => x.Sid == currentSectorLevelSid);
            }
            else
            {
                sectorLevelScheme = scheme.SectorLevels.Single(x => x.IsStart);
            }

            _humanPlayer.SectorSid = scheme.Sid;
            _humanPlayer.SectorLevelSid = sectorLevelScheme.Sid;

            CurrentSector = await _generator.GenerateDungeonAsync(sectorLevelScheme).ConfigureAwait(false);
            CurrentSector.Scheme = scheme;
        }
    }
}
