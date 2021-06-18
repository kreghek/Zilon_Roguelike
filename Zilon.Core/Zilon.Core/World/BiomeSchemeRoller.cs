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
            var openSchemeSids = Enumerable.Range(1, 20).Select(x => "rat-hole").ToArray();

            var dungeonSchemeSids = new[]
            {
                "rat-hole",
                "rat-kingdom",
                "demon-dungeon",
                "demon-lair",
                "crypt",
                "elder-place",
                "genomass-cave"
            };

            var totalLocationSchemeSids = openSchemeSids.Concat(dungeonSchemeSids).ToArray();

            var rolledLocationSchemeSid = _dice.RollFromList(totalLocationSchemeSids);
            var rolledLocationScheme = _schemeService.GetScheme<ILocationScheme>(rolledLocationSchemeSid);

            return rolledLocationScheme;
        }
    }
}