﻿using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Diseases;
using Zilon.Core.Graphs;
using Zilon.Core.PersonGeneration;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Реализация генератора монстров.
    /// </summary>
    /// <seealso cref="IMonsterGenerator" />
    public class MonsterGenerator : IMonsterGenerator
    {
        private readonly IActorTaskSource<ISectorTaskSourceContext> _actorTaskSource;
        private readonly IMonsterGeneratorRandomSource _generatorRandomSource;
        private readonly IMonsterPersonFactory _monsterFactory;
        private readonly ISchemeService _schemeService;

        /// <summary>
        /// Создаёт экземпляр <see cref="MonsterGenerator" />.
        /// </summary>
        /// <param name="schemeService"> Сервис схем. </param>
        /// <param name="generatorRandomSource"> Источник рандома для генератора. </param>
        public MonsterGenerator(ISchemeService schemeService,
            IMonsterPersonFactory monsterFactory,
            IMonsterGeneratorRandomSource generatorRandomSource,
            IActorTaskSource<ISectorTaskSourceContext> actorTaskSource)
        {
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _monsterFactory = monsterFactory ?? throw new ArgumentNullException(nameof(monsterFactory));
            _generatorRandomSource =
                generatorRandomSource ?? throw new ArgumentNullException(nameof(generatorRandomSource));
            _actorTaskSource = actorTaskSource;
        }

        private static IActor CreateMonster(IActorManager actorManager, MonsterPerson person, IGraphNode startNode,
            IActorTaskSource<ISectorTaskSourceContext> actorTaskSource)
        {
            var actor = new Actor(person, actorTaskSource, startNode);
            actorManager.Add(actor);
            return actor;
        }

        private IActor CreateMonster(IActorManager actorManager, IMonsterScheme monsterScheme, IGraphNode startNode,
            IActorTaskSource<ISectorTaskSourceContext> actorTaskSource)
        {
            var person = _monsterFactory.Create(monsterScheme);
            var actor = new Actor(person, actorTaskSource, startNode);
            actorManager.Add(actor);
            return actor;
        }

        private void CreateMonstersForRegion(ISector sector, ISectorSubScheme sectorScheme,
            List<IActor> resultMonsterActors, MapRegion region, int[] rarityCounter)
        {
            var regionNodes = region.Nodes.OfType<HexNode>();
            var staticObjectsNodes = sector.StaticObjectManager.Items.Select(x => x.Node);
            var nodesWithTransitions = sector.Map.Transitions.Keys.ToArray();
            var availableMonsterNodes = regionNodes.Except(staticObjectsNodes).Except(nodesWithTransitions);

            var freeNodes = new List<IGraphNode>(availableMonsterNodes);

            var monsterCount = _generatorRandomSource.RollRegionCount(
                sectorScheme.MinRegionMonsterCount,
                sectorScheme.RegionMonsterCount);

            for (var i = 0; i < monsterCount; i++)
            {
                // если в комнате все места заняты
                if (!freeNodes.Any())
                {
                    break;
                }

                var rollIndex = _generatorRandomSource.RollNodeIndex(freeNodes.Count);
                var monsterNode = freeNodes[rollIndex];

                var monster = RollRarityAndCreateMonster(sector, sectorScheme, monsterNode, rarityCounter);

                freeNodes.Remove(monster.Node);
                resultMonsterActors.Add(monster);
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
            var availableSchemeSids = GetMonsterSidsByRarity(sectorScheme, currentRarity);

            if (availableSchemeSids is null)
            {
                throw new InvalidOperationException("Не удалось выбрать доступные схемы для монстров.");
            }

            return availableSchemeSids.Where(x => x != null).Select(x => x!).ToArray();
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

        private static IEnumerable<string?>? GetMonsterSidsByRarity(ISectorSubScheme sectorScheme, int currentRarity)
        {
            return currentRarity switch
            {
                0 => sectorScheme.RegularMonsterSids,
                1 => sectorScheme.RareMonsterSids ?? sectorScheme.RegularMonsterSids,
                2 => sectorScheme.ChampionMonsterSids ??
                     sectorScheme.RareMonsterSids ?? sectorScheme.RegularMonsterSids,
                _ => throw new InvalidOperationException()
            };
        }

        private void RollInfections(ISector sector, List<IActor> resultMonsterActors)
        {
            var diseaseMonsters = resultMonsterActors.Select(x => x.Person).ToArray();
            if (sector.Diseases?.Any() == true)
            {
                foreach (var disease in sector.Diseases)
                {
                    var rollInfectedMonsters = _generatorRandomSource.RollInfectedMonsters(diseaseMonsters, 0.1f);
                    foreach (var monster in rollInfectedMonsters)
                    {
                        SetMonsterInfection(monster, disease);
                    }
                }
            }
        }

        private IActor RollRarityAndCreateMonster(ISector sector, ISectorSubScheme sectorScheme, IGraphNode monsterNode,
            int[] rarityCounter)
        {
            var rarityMaxCounter = new[] { -1, 10, 1 };

            var currentRarity = GetMonsterRarity(rarityCounter, rarityMaxCounter);
            var availableSchemeSids = GetAvailableSchemeSids(sectorScheme, currentRarity);

            var availableMonsterSchemes = availableSchemeSids.Select(x => _schemeService.GetScheme<IMonsterScheme>(x));

            var monsterScheme = _generatorRandomSource.RollMonsterScheme(availableMonsterSchemes);

            var monster = CreateMonster(sector.ActorManager, monsterScheme, monsterNode, _actorTaskSource);

            return monster;
        }

        private static void SetMonsterInfection(IPerson monster, IDisease disease)
        {
            monster.GetModule<IDiseaseModule>().Infect(disease);
        }

        /// <summary>Создаёт монстров в секторе по указанной схеме.</summary>
        /// <param name="sector">Целевой сектор.</param>
        /// <param name="monsterRegions">Регионы сектора, где могут быть монстры.</param>
        /// <param name="sectorScheme">Схема сектора. Отсюда берутся параметры генерации монстров.</param>
        public void CreateMonsters(ISector sector,
            IEnumerable<MapRegion> monsterRegions,
            ISectorSubScheme sectorScheme)
        {
            if (sector is null)
            {
                throw new ArgumentNullException(nameof(sector));
            }

            if (monsterRegions is null)
            {
                throw new ArgumentNullException(nameof(monsterRegions));
            }

            if (sectorScheme is null)
            {
                throw new ArgumentNullException(nameof(sectorScheme));
            }

            var resultMonsterActors = new List<IActor>();
            var rarityCounter = new int[3];

            foreach (var region in monsterRegions)
            {
                CreateMonstersForRegion(sector, sectorScheme, resultMonsterActors, region, rarityCounter);
            }

            // Инфицируем монстров, если в секторе есть болезни.
            RollInfections(sector, resultMonsterActors);
        }

        public void CreateMonsters(ISector sector, IEnumerable<MapRegion> monsterRegions,
            IEnumerable<MonsterPerson> monsterPersons)
        {
            if (sector == null)
            {
                throw new ArgumentNullException(nameof(sector));
            }

            if (monsterRegions == null)
            {
                throw new ArgumentNullException(nameof(monsterRegions));
            }

            if (monsterPersons == null)
            {
                throw new ArgumentNullException(nameof(monsterPersons));
            }

            var freeNodes = new List<IGraphNode>(monsterRegions.SelectMany(x => x.Nodes));

            foreach (var monsterPerson in monsterPersons)
            {
                var rollIndex = _generatorRandomSource.RollNodeIndex(freeNodes.Count);

                var monsterNode = freeNodes[rollIndex];
                var monster = CreateMonster(sector.ActorManager, monsterPerson, monsterNode, _actorTaskSource);

                freeNodes.Remove(monster.Node);
            }
        }
    }
}