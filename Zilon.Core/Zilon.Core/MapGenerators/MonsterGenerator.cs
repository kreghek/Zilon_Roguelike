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
            var rarityMaxCounter = new int[3] { -1, 5, 1 };

            foreach (var region in monsterRegions)
            {
                var monsterCount = _generatorRandomSource.RollCount();

                for (int i = 0; i < monsterCount; i++)
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
                        }
                    }
                    
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

                    var availableMonsterSchemes = availableSchemeSids.Select(x => _schemeService.GetScheme<IMonsterScheme>(x));

                    var monsterScheme = _generatorRandomSource.RollMonsterScheme(availableMonsterSchemes);

                    //TODO Учесть вероятность, что монстр может инстанцироваться на сундук
                    if (i == 0)
                    {
                        // В каждую комнату генерируем по 2 монстра
                        // первый ходит по маршруту

                        var startNode1 = (HexNode)region.Nodes.FirstOrDefault();
                        var actor1 = CreateMonster(monsterScheme, startNode1, monsterGeneratorOptions.BotPlayer);

                        var finishPatrolNode = region.Nodes.Last();
                        var patrolRoute = new PatrolRoute(startNode1, finishPatrolNode);
                        sector.PatrolRoutes[actor1] = patrolRoute;
                    }
                    else
                    {
                        // второй произвольно бродит

                        var startNode2 = (HexNode)region.Nodes.Skip(3).FirstOrDefault();
                        CreateMonster(monsterScheme, startNode2, monsterGeneratorOptions.BotPlayer);
                    }
                }
            }
        }

        private IActor CreateMonster(IMonsterScheme monsterScheme, HexNode startNode, IBotPlayer botPlayer)
        {
            var person = new MonsterPerson(monsterScheme, _survivalRandomSource);
            var actor = new Actor(person, botPlayer, startNode);
            _actorManager.Add(actor);
            return actor;
        }
    }
}
