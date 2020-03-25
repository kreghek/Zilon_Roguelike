using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Реализация генератора сектора для подземелий.
    /// </summary>
    /// <seealso cref="ISectorGenerator" />
    public class SectorGenerator : ISectorGenerator
    {
        private readonly IChestGenerator _chestGenerator;
        private readonly IBotPlayer _botPlayer;
        private readonly ICitizenGenerator _citizenGenerator;
        private readonly IMapFactorySelector _mapFactorySelector;
        private readonly ISectorFactory _sectorFactory;
        private readonly IMonsterGenerator _monsterGenerator;

        /// <summary>
        /// Создаёт экземпляр <see cref="SectorGenerator"/>.
        /// </summary>
        /// <param name="mapFactorySelector"> Сервис для выбора фабрики для создания карты. </param>
        /// <param name="sectorFactory"> Фабрика сектора. </param>
        /// <param name="monsterGenerator"> Генератор монстров для подземелий. </param>
        /// <param name="chestGenerator"> Генератор сундуков для подземеоий </param>
        /// <param name="citizenGenerator"> Генератор жителей в городском квартале. </param>
        /// <param name="botPlayer"> Игрок, управляющий монстрами, мирными жителями. </param>
        public SectorGenerator(
            IMapFactorySelector mapFactorySelector,
            ISectorFactory sectorFactory,
            IMonsterGenerator monsterGenerator,
            IChestGenerator chestGenerator,
            ICitizenGenerator citizenGenerator,
            IBotPlayer botPlayer)
        {
            _mapFactorySelector = mapFactorySelector;
            _sectorFactory = sectorFactory;
            _monsterGenerator = monsterGenerator;
            _chestGenerator = chestGenerator;
            _botPlayer = botPlayer;
            _citizenGenerator = citizenGenerator;
        }

        /// <summary>
        /// Создаёт экземпляр сектора подземелий с указанными параметрами.
        /// </summary>
        /// <param name="sectorNode"> Схема генерации сектора. </param>
        /// <returns> Возвращает экземпляр сектора. </returns>
        public async Task<ISector> GenerateAsync(ISectorNode sectorNode)
        {
            if (sectorNode is null)
            {
                throw new System.ArgumentNullException(nameof(sectorNode));
            }

            var mapFactory = _mapFactorySelector.GetMapFactory(sectorNode);

            var transitions = MapFactoryHelper.CreateTransitions(sectorNode);

            var sectorFactoryOptions = new SectorMapFactoryOptions { 
                OptionsSubScheme = sectorNode.SectorScheme.MapGeneratorOptions,
                Transitions = transitions
            };

            var map = await mapFactory.CreateAsync(sectorFactoryOptions).ConfigureAwait(false);

            var sector = _sectorFactory.Create(map);

            var gameObjectRegions = map.Regions.Where(x => !x.IsStart).ToArray();

            var sectorScheme = sectorNode.SectorScheme;

            var chestRegions = gameObjectRegions.Where(x => x.Nodes.Length > 4);
            _chestGenerator.CreateChests(map, sectorScheme, chestRegions);

            var monsterRegions = gameObjectRegions.ToArray();
            _monsterGenerator.CreateMonsters(sector,
                _botPlayer,
                monsterRegions,
                sectorScheme);

            return sector;
        }
    }
}
