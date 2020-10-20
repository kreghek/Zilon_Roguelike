using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.Tactics
{
    public class InfiniteSectorManager : ISectorManager
    {
        private const string LOCATION_SID = "infinity-dungeon";

        private readonly ISectorGenerator _generator;
        private readonly ISchemeService _schemeService;
        private readonly IBiomeInitializer _biomeInitializer;
        private readonly HumanPlayer _humanPlayer;

        public InfiniteSectorManager(ISectorGenerator generator,
            ISchemeService schemeService,
            IBiomeInitializer biomeInitializer,
            HumanPlayer humanPlayer)
        {
            _generator = generator ?? throw new System.ArgumentNullException(nameof(generator));
            _schemeService = schemeService ?? throw new System.ArgumentNullException(nameof(schemeService));
            _biomeInitializer = biomeInitializer ?? throw new System.ArgumentNullException(nameof(biomeInitializer));
            _humanPlayer = humanPlayer;
        }

        public ISector CurrentSector { get; private set; }

        public async Task CreateSectorAsync()
        {
            if (_humanPlayer.SectorNode is null)
            {
                var scheme = _schemeService.GetScheme<ILocationScheme>(LOCATION_SID);

                var biome = await _biomeInitializer.InitBiomeAsync(scheme).ConfigureAwait(false);

                var sectorNode = biome.Sectors.First();

                _humanPlayer.BindSectorNode(sectorNode);

                CurrentSector = sectorNode.Sector;
            }
            else
            {
                await _biomeInitializer.MaterializeLevelAsync(_humanPlayer.SectorNode).ConfigureAwait(false);
                CurrentSector = _humanPlayer.SectorNode.Sector;
            }
        }
    }
}