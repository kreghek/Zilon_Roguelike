using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Реализация генератора монстров.
    /// </summary>
    /// <seealso cref="Zilon.Core.MapGenerators.IMonsterGenerator" />
    public class MonsterGenerator : IMonsterGenerator
    {
        private readonly ISchemeService _schemeService;
        private readonly IMonsterGeneratorRandomSource _generatorRandomSource;
        private readonly IActorManager _actorManager;
        private readonly IPropContainerManager _propContainerManager;

        /// <summary>
        /// Создаёт экземпляр <see cref="MonsterGenerator"/>.
        /// </summary>
        /// <param name="schemeService"> Сервис схем. </param>
        /// <param name="generatorRandomSource"> Источник рандома для генератора. </param>
        /// <param name="actorManager"> Менеджер актёров, в который размещаются монстры. </param>
        public MonsterGenerator(ISchemeService schemeService,
            IMonsterGeneratorRandomSource generatorRandomSource,
            IActorManager actorManager,
            IPropContainerManager propContainerManager)
        {
            _schemeService = schemeService;
            _generatorRandomSource = generatorRandomSource;
            _actorManager = actorManager;
            _propContainerManager = propContainerManager;
        }

        /// <summary>Создаёт монстров в секторе по указанной схеме.</summary>
        /// <param name="sector">Целевой сектор.</param>
        /// <param name="monsterPlayer">Бот, управляющий монстрами. По сути, команда монстров.</param>
        /// <param name="monsterRegions">Регионы сектора, где могут быть монстры.</param>
        /// <param name="sectorScheme">Схема сектора. Отсюда берутся параметры генерации монстров.</param>
        public void CreateMonsters(ISector sector,
            IBotPlayer monsterPlayer,
            IEnumerable<MapRegion> monsterRegions,
            ISectorSubScheme sectorScheme)
        {
            var rarityCounter = new int[3];
            var rarityMaxCounter = new[] { -1, 10, 1 };

            foreach (var region in monsterRegions)
            {
                var regionNodes = region.Nodes;
                var containerNodes = _propContainerManager.Items.Select(x => x.Node);
                var availableMonsterNodes = regionNodes.Except(containerNodes);

                var freeNodes = new List<IMapNode>(availableMonsterNodes);

                var monsterCount = _generatorRandomSource.RollRegionCount(
                    sectorScheme.MinRegionMonsterCount,
                    sectorScheme.RegionMonsterCount);

                for (int i = 0; i < monsterCount; i++)
                {
                    // если в комнате все места заняты
                    if (!freeNodes.Any())
                    {
                        break;
                    }

                    var currentRarity = GetMonsterRarity(rarityCounter, rarityMaxCounter);
                    var availableSchemeSids = GetAvailableSchemeSids(sectorScheme, currentRarity);

                    var availableMonsterSchemes = availableSchemeSids.Select(x => _schemeService.GetScheme<IMonsterScheme>(x));

                    var monsterScheme = _generatorRandomSource.RollMonsterScheme(availableMonsterSchemes);

                    //TODO Восстановить патруллирование марштуров позже
                    // первый монстр ходит по маршруту
                    // остальные бродят произвольно
                    //if (i == 0)
                    //{
                    //    // генерируем маршрут обхода
                    //    var startPatrolNode = region.Nodes.First();
                    //    var endPatrolNode = region.Nodes.Last();

                    //    // генерируем моснтра
                    //    var patrolRoute = new PatrolRoute(startPatrolNode, endPatrolNode);
                    //    var monster = CreateMonster(monsterScheme, startPatrolNode, monsterPlayer);
                    //    sector.PatrolRoutes[monster] = patrolRoute;

                    //    freeNodes.Remove(monster.Node);
                    //}
                    //else
                    //{
                        var rollIndex = _generatorRandomSource.RollNodeIndex(freeNodes.Count);
                        var monsterNode = freeNodes[rollIndex];
                        var monster = CreateMonster(monsterScheme, monsterNode, monsterPlayer);

                        freeNodes.Remove(monster.Node);
                    //}
                }
            }
        }

        /// <summary>
        /// Получение доступных схем моснтров на основе указанной редкости монстра.
        /// </summary>
        /// <param name="sectorScheme"> Настройки генерации монстров. </param>
        /// <param name="currentRarity"> Целевой уровень редкости монстра. </param>
        /// <returns> Возвращает набор строк, являющихся идентификаторами схем монстров. </returns>
        private static IEnumerable<string> GetAvailableSchemeSids(
            ISectorSubScheme sectorScheme,
            int currentRarity)
        {
            IEnumerable<string> availableSchemeSids;
            switch (currentRarity)
            {
                case 0:
                    availableSchemeSids = sectorScheme.RegularMonsterSids;
                    break;

                case 1:
                    availableSchemeSids = sectorScheme.RareMonsterSids ??
                        sectorScheme.RegularMonsterSids;
                    break;

                case 2:
                    availableSchemeSids = sectorScheme.ChampionMonsterSids ??
                        sectorScheme.RareMonsterSids ??
                        sectorScheme.RegularMonsterSids;
                    break;

                default:
                    throw new InvalidOperationException();
            }

            if (availableSchemeSids == null)
            {
                throw new InvalidOperationException("Не удалось выбрать доступные схемы для монстров.");
            }

            return availableSchemeSids;
        }

        /// <summary>
        /// Получение редкости текущего монстра.
        /// </summary>
        /// <param name="rarityCounter"> Систояние счётчиков редкости. </param>
        /// <param name="rarityMaxCounter"> Максимальные значения счётчиков редкости монстров в секторе. </param>
        /// <returns> Возвращает целочисленное значение, представляющее редкость монстра. </returns>
        /// <remarks>
        /// 0 - обычный.
        /// 1 - редкий.
        /// 2 - чемпион (уникальный, босс).
        /// </remarks>
        private int GetMonsterRarity(int[] rarityCounter, int[] rarityMaxCounter)
        {
            var rarity = _generatorRandomSource.RollRarity();

            //Снижать редкость, если лимит текущей редкости исчерпан
            var currentRarity = rarity;
            while (currentRarity > 0)
            {
                var currentRarityCounter = rarityCounter[currentRarity];
                var maxRarityCounter = rarityMaxCounter[currentRarity];
                if (currentRarityCounter >= maxRarityCounter)
                {
                    currentRarity--;
                }
                else
                {
                    rarityCounter[currentRarity]++;
                    break;
                }
            }

            return currentRarity;
        }

        private IActor CreateMonster(MonsterPerson person, IMapNode startNode, IBotPlayer botPlayer)
        {
            var actor = new Actor(person, botPlayer, startNode);
            _actorManager.Add(actor);
            return actor;
        }

        private IActor CreateMonster(IMonsterScheme monsterScheme, IMapNode startNode, IBotPlayer botPlayer)
        {
            var person = new MonsterPerson(monsterScheme);
            var actor = new Actor(person, botPlayer, startNode);
            _actorManager.Add(actor);
            return actor;
        }

        public void CreateMonsters(ISector sector, IBotPlayer monsterPlayer, IEnumerable<MapRegion> monsterRegions, IEnumerable<MonsterPerson> monsterPersons)
        {
            if (sector == null)
            {
                throw new ArgumentNullException(nameof(sector));
            }

            if (monsterPlayer == null)
            {
                throw new ArgumentNullException(nameof(monsterPlayer));
            }

            if (monsterRegions == null)
            {
                throw new ArgumentNullException(nameof(monsterRegions));
            }

            if (monsterPersons == null)
            {
                throw new ArgumentNullException(nameof(monsterPersons));
            }

            var freeNodes = new List<IMapNode>(monsterRegions.SelectMany(x=>x.Nodes));

            foreach (var monsterPerson in monsterPersons)
            {
                var rollIndex = _generatorRandomSource.RollNodeIndex(freeNodes.Count);

                var monsterNode = freeNodes[rollIndex];
                var monster = CreateMonster(monsterPerson, monsterNode, monsterPlayer);

                freeNodes.Remove(monster.Node);
            }
        }
    }
}
