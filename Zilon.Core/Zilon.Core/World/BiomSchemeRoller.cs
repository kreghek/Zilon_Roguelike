using System;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    public class BiomSchemeRoller : IBiomSchemeRoller
    {
        private readonly ISchemeService _schemeService;
        private readonly IDice _dice;

        public BiomSchemeRoller(ISchemeService schemeService, IDice dice)
        {
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));
        }

        public ILocationScheme Roll()
        {
            var locationSchemeSids = new[]
                {
                "rat-hole",
                "rat-kingdom",
                "demon-dungeon",
                "demon-lair",
                "crypt",
                "elder-place",
                "genomass-cave"
                };

            var rolledLocationSchemeSid = _dice.RollFromList(locationSchemeSids);
            var rolledLocationScheme = _schemeService.GetScheme<ILocationScheme>(rolledLocationSchemeSid);

            return rolledLocationScheme;
        }
    }
}
