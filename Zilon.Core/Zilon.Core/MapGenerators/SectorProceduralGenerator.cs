using System.Linq;

using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators
{
    public class SectorProceduralGenerator : ISectorProceduralGenerator
    {

        private readonly IMapFactory _mapFactory;
        private readonly IChestGenerator _chestGenerator;
        private readonly ISectorFactory _sectorFactory;
        private readonly IMonsterGenerator _monsterGenerator;

        public SectorProceduralGenerator(
            IMapFactory mapFactory,
            ISectorFactory sectorFactory,
            IMonsterGenerator monsterGenerator,
            IChestGenerator chestGenerator
            )
        {
            _mapFactory = mapFactory;
            _sectorFactory = sectorFactory;
            _monsterGenerator = monsterGenerator;
            _chestGenerator = chestGenerator;
        }

        public ISector Generate(ISectorGeneratorOptions options)
        {
            var map = _mapFactory.Create();

            var sector = _sectorFactory.Create(map);

            var proceduralOptions = (SectorProceduralGeneratorOptions)options;

            var monsterRegions = map.Regions.Where(x => x != map.StartRegion);

            _monsterGenerator.CreateMonsters(sector,
                monsterRegions,
                proceduralOptions.MonsterGeneratorOptions);

            _chestGenerator.CreateChests(map, monsterRegions);

            return sector;
        }
    }
}
