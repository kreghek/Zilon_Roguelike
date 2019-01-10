using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public class MonsterGenerator : IMonsterGenerator
    {
        private readonly ISchemeService _schemeService;
        private readonly IMonsterGeneratorRandomSource _generatorRandomSource;
        private readonly IActorManager _actorManager;
        private readonly ISurvivalRandomSource _survivalRandomSource;

        public MonsterGenerator(ISchemeService schemeService,
            IMonsterGeneratorRandomSource generatorRandomSource,
            IActorManager actorManager,
            ISurvivalRandomSource survivalRandomSource)
        {
            _schemeService = schemeService;
            _generatorRandomSource = generatorRandomSource;
            _actorManager = actorManager;
            _survivalRandomSource = survivalRandomSource;
        }

        public void CreateMonsters(ISector sector,
            IEnumerable<MapRegion> monsterRegions,
            IMonsterGeneratorOptions monsterGeneratorOptions)
        {
            var rarityCounter = new int[3];
            var rarityMaxCounter = new int[3] { -1, 10, 1 };

            foreach (var region in monsterRegions)
            {
                var freeNodes = new List<IMapNode>(region.Nodes);

                var monsterCount = _generatorRandomSource.RollCount();

                for (int i = 0; i < monsterCount; i++)
                {
                    // если в комнате все места заняты
                    if (!freeNodes.Any())
                    {
                        break;
                    }

                    var currentRarity = GetMonsterRarity(rarityCounter, rarityMaxCounter);
                    var availableSchemeSids = GetAvailableSchemeSids(monsterGeneratorOptions, currentRarity);

                    var availableMonsterSchemes = availableSchemeSids.Select(x => _schemeService.GetScheme<IMonsterScheme>(x));

                    var monsterScheme = _generatorRandomSource.RollMonsterScheme(availableMonsterSchemes);

                    // первый монстр ходит по маршруту
                    // остальные бродят произвольно
                    if (i == 0)
                    {
                        // генерируем маршрут обхода
                        var startPatrolNode = region.Nodes.First();
                        var endPatrolNode = region.Nodes.Last();

                        // генерируем моснтра
                        var patrolRoute = new PatrolRoute(startPatrolNode, endPatrolNode);
                        var monster = CreateMonster(monsterScheme, startPatrolNode, monsterGeneratorOptions.BotPlayer);
                        sector.PatrolRoutes[monster] = patrolRoute;

                        freeNodes.Remove(monster.Node);
                    }
                    else
                    {
                        var rollIndex = _generatorRandomSource.RollNodeIndex(freeNodes.Count);
                        var monsterNode = freeNodes[rollIndex];
                        var monster = CreateMonster(monsterScheme, monsterNode, monsterGeneratorOptions.BotPlayer);

                        freeNodes.Remove(monster.Node);
                    }
                }
            }
        }

        /// <summary>
        /// Получение доступных схем моснтров на основе указанной редкости монстра.
        /// </summary>
        /// <param name="monsterGeneratorOptions"> Настройки генерации монстров. </param>
        /// <param name="currentRarity"> Целевой уровень редкости монстра. </param>
        /// <returns> Возвращает набор строк, являющихся идентификаторами схем монстров. </returns>
        private static IEnumerable<string> GetAvailableSchemeSids(IMonsterGeneratorOptions monsterGeneratorOptions, int currentRarity)
        {
            IEnumerable<string> availableSchemeSids;
            switch (currentRarity)
            {
                case 0:
                    availableSchemeSids = monsterGeneratorOptions.RegularMonsterSids;
                    break;

                case 1:
                    availableSchemeSids = monsterGeneratorOptions.RareMonsterSids ??
                        monsterGeneratorOptions.RegularMonsterSids;
                    break;

                case 2:
                    availableSchemeSids = monsterGeneratorOptions.ChampionMonsterSids ??
                        monsterGeneratorOptions.RareMonsterSids ??
                        monsterGeneratorOptions.RegularMonsterSids;
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

        private IActor CreateMonster(IMonsterScheme monsterScheme, IMapNode startNode, IBotPlayer botPlayer)
        {
            var person = new MonsterPerson(monsterScheme, _survivalRandomSource);
            var actor = new Actor(person, botPlayer, startNode);
            _actorManager.Add(actor);
            return actor;
        }
    }
}
