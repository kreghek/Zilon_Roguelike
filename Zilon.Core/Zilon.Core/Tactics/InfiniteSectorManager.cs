using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics
{
    public class InfiniteSectorManager : ISectorManager
    {
        private const string LOCATION_SID = "infinity-dungeon";

        private readonly ISectorGenerator _generator;
        private readonly ISchemeService _schemeService;

        public InfiniteSectorManager(ISectorGenerator generator,
            ISchemeService schemeService)
        {
            _generator = generator;
            _schemeService = schemeService;
        }

        public ISector CurrentSector { get; private set; }

        public async Task CreateSectorAsync()
        {
            var scheme = _schemeService.GetScheme<ILocationScheme>(LOCATION_SID);

            var sectorLevelScheme = scheme.SectorLevels[0];

            CurrentSector = await _generator.GenerateAsync(sectorLevelScheme).ConfigureAwait(false);
            CurrentSector.Scheme = scheme;
        }
    }
}
