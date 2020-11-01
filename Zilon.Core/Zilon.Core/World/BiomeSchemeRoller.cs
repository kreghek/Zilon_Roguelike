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
            var locationSchemeSids = new[]
            {
                "rat-hole", "rat-kingdom", "demon-dungeon", "demon-lair", "crypt", "elder-place", "genomass-cave"
            };

            var rolledLocationSchemeSid = _dice.RollFromList(locationSchemeSids);
            ILocationScheme rolledLocationScheme = _schemeService.GetScheme<ILocationScheme>(rolledLocationSchemeSid);

            return rolledLocationScheme;
        }
    }
}