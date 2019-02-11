using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Реализация менеджера сектора.
    /// </summary>
    /// <seealso cref="ISectorManager" />
    public class SectorManager : ISectorManager
    {
        private const string IntrolLocationSid = "intro";
        private readonly IWorldManager _worldManager;
        private readonly ISectorGenerator _generator;
        private readonly HumanPlayer _humanPlayer;
        private readonly ISchemeService _schemeService;

        public SectorManager(IWorldManager worldManager,
            ISectorGenerator generator,
            HumanPlayer humanPlayer,
            ISchemeService schemeService)
        {
            _worldManager = worldManager ?? throw new ArgumentNullException(nameof(worldManager));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _humanPlayer = humanPlayer ?? throw new ArgumentNullException(nameof(humanPlayer));
            _schemeService = schemeService;
        }

        /// <summary>
        /// Текущий сектор.
        /// </summary>
        public ISector CurrentSector { get; private set; }

        /// <summary>
        /// Создаёт текущий сектор по указанному генератору и настройкам.
        /// </summary>
        public async Task CreateSectorAsync()
        {
            var regionNode = _humanPlayer.GlobeNode;

            ILocationScheme scheme = null;
            if (_humanPlayer.GlobeNode == null)
            {
                scheme = _schemeService.GetScheme<ILocationScheme>(IntrolLocationSid);
            }
            else
            {
                scheme = _humanPlayer.GlobeNode.Scheme;
            }

            if (scheme.SectorLevels != null)
            {
                ISectorSubScheme sectorLevelScheme;
                if (_humanPlayer.SectorSid == null)
                {
                    sectorLevelScheme = scheme.SectorLevels.SingleOrDefault(x => x.IsStart);
                }
                else
                {
                    sectorLevelScheme = scheme.SectorLevels.SingleOrDefault(x => x.Sid == _humanPlayer.SectorSid);
                }
                
                CurrentSector = await _generator.GenerateDungeonAsync(sectorLevelScheme);
            }
            else if (regionNode.IsTown)
            {
                CurrentSector = await _generator.GenerateTownQuarterAsync(_worldManager.Globe, regionNode);
            }
            else
            {
                CurrentSector = await _generator.GenerateWildAsync(_worldManager.Globe, regionNode);
            }
        }
    }
}
