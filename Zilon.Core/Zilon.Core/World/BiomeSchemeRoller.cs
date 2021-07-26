using System;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    public class BiomeSchemeRoller : IBiomeSchemeRoller
    {
        private readonly IDice _dice;
        private readonly ISchemeService _schemeService;

        public BiomeSchemeRoller(ISchemeService schemeService, IDice dice)
        {
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));
        }

        public ILocationScheme Roll()
        {
            var globeNodeMeatSchemeSids = Enumerable.Range(1, 7).Select(x => "globe-node-meat").ToArray();
            var globeNodeWaterSchemeSids = Enumerable.Range(1, 7).Select(x => "globe-node-water").ToArray();
            var globeNodeMedkitSchemeSids = Enumerable.Range(1, 7).Select(x => "globe-node-medkit").ToArray();

            var dungeonSchemeSids = new[]
            {
                "dungeon",
                "elder-temple"
            };

            var totalLocationSchemeSids = globeNodeMeatSchemeSids.Concat(globeNodeWaterSchemeSids).Concat(globeNodeMedkitSchemeSids).Concat(dungeonSchemeSids).ToArray();

            var rolledLocationSchemeSid = _dice.RollFromList(totalLocationSchemeSids);
            var rolledLocationScheme = _schemeService.GetScheme<ILocationScheme>(rolledLocationSchemeSid);

            return rolledLocationScheme;
        }
    }
}