using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class PersonPerkInitializator : IPersonPerkInitializator
    {
        private const int START_TRAIT_MIN_COUNT = 2;
        private const int START_TRAIT_MAX_COUNT = 5;

        private readonly IDice _dice;
        private readonly ISchemeService _schemeService;

        public PersonPerkInitializator(IDice dice, ISchemeService schemeService)
        {
            _dice = dice;
            _schemeService = schemeService;
        }

        public IPerk[] Generate()
        {
            var allBuildInPerks = _schemeService.GetSchemes<IPerkScheme>().Where(x => x.IsBuildIn).ToArray();

            var maxPerkCount = Math.Min(allBuildInPerks.Length, START_TRAIT_MAX_COUNT);
            var minPerkCount = Math.Min(maxPerkCount, START_TRAIT_MIN_COUNT);

            var startPerkCount = _dice.Roll(minPerkCount, maxPerkCount);

            var traitList = new List<IPerk>();
            var openTraitSchemeList = new List<IPerkScheme>(allBuildInPerks);

            for (var i = 0; i < startPerkCount; i++)
            {
                var rolledTraitScheme = _dice.RollFromList(openTraitSchemeList);
                openTraitSchemeList.Remove(rolledTraitScheme);

                var traitPerk = new Perk
                {
                    Scheme = rolledTraitScheme
                };

                traitList.Add(traitPerk);
            }

            return traitList.ToArray();
        }
    }
}