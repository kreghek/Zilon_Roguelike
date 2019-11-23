using System;

using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    /// <summary>
    /// Базовая реализация сервиса для работы с глобальным миром.
    /// </summary>
    public sealed class WorldManager : IWorldManager
    {
        private const int SPAWN_SUCCESS_ROLL = 5;
        private const int MONSTER_NODE_LIMIT = 100;

        private readonly ISchemeService _schemeService;
        private readonly IDice _dice;

        public WorldManager(ISchemeService schemeService, IDice dice)
        {
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));

            Regions = new Dictionary<TerrainCell, GlobeRegion>();
        }

        /// <summary>
        /// Глобальная карта.
        /// </summary>
        public Globe Globe { get; set; }

        /// <summary>
        /// Текущие сгенерированые провинции относительно ячеек глобальной карты.
        /// </summary>
        public Dictionary<TerrainCell, GlobeRegion> Regions { get; }

        /// <summary>
        /// История генерации мира.
        /// </summary>
        public GlobeGenerationHistory GlobeGenerationHistory { get; set; }

        /// <summary>
        /// Обновление состояния узлов провинции.
        /// </summary>
        /// <param name="region">Провинция, которая обновляется.</param>
        public void UpdateRegionNodes(GlobeRegion region)
        {
            // Подсчитываем узлы, занятые монстрами.
            // Это делаем для того, чтобы следить за плотностью моснтров в секторе.

            var nodeWithMonsters = region.RegionNodes.Where(x => x.MonsterState != null);

            var monsterLimitIsReached = nodeWithMonsters.Count() >= MONSTER_NODE_LIMIT;

            // Наборы монстров для генерации в узлах.
            var monsterSets = CreateMonsterSets();

            foreach (var node in region.RegionNodes)
            {
                if (node.MonsterState == null)
                {
                    if (!monsterLimitIsReached)
                    {
                        var spawnMonsterRoll = _dice.RollD6();

                        if (spawnMonsterRoll >= SPAWN_SUCCESS_ROLL)
                        {
                            CreateNodeMonsterState(monsterSets, node);
                        }
                    }
                }
                else
                {
                    // В метод передём только состояние, как минимальные данные,
                    // которые там нужны.
                    // Внутри метода он может зануляться, поэтому передаём по ссылке.
                    var monsterState = node.MonsterState;
                    UpdateNodeMonsterState(ref monsterState);
                    node.MonsterState = monsterState;
                }
            }
        }

        private void UpdateNodeMonsterState(ref GlobeRegionNodeMonsterState MonsterState)
        {
            // И состояния ментров отсеиваем всех мёртвых монстров.
            var aliveMonsterPersons = new List<MonsterPerson>();
            foreach (var monsterPerson in MonsterState.MonsterPersons)
            {
                if (!monsterPerson.CheckIsDead())
                {
                    aliveMonsterPersons.Add(monsterPerson);
                }
            }

            // Если игрок убил всех монстров в узле, то подчищаем состояние монстров в узле.
            // Потому что null означает, что туда можно сгенерировать новых монстров
            // + снижает текущий счётчик узлов с монстрами.
            if (!aliveMonsterPersons.Any())
            {
                MonsterState = null;
            }
        }

        private void CreateNodeMonsterState(MonsterSet[] monsterSets, GlobeRegionNode node)
        {
            var monsterSetRoll = _dice.Roll(0, monsterSets.Count() - 1);
            var rolledMonsterSet = monsterSets[monsterSetRoll];

            var monsterCount = rolledMonsterSet.MonsterSchemes.Count();

            node.MonsterState = new GlobeRegionNodeMonsterState
            {
                MonsterPersons = new MonsterPerson[monsterCount]
            };

            // Генерируем персонажей монстров.
            for (var monsterIndex = 0; monsterIndex < monsterCount; monsterIndex++)
            {
                var monsterScheme = rolledMonsterSet.MonsterSchemes[monsterIndex];
                var person = new MonsterPerson(monsterScheme);
                node.MonsterState.MonsterPersons[monsterIndex] = person;
            }
        }

        //TODO Вынести в отдельную схему.
        // Решить, куда лучше. В подсхему сектора узла провинции или сделать отдельно.
        private MonsterSet[] CreateMonsterSets()
        {
            return new[] {
                new MonsterSet{
                    MonsterSchemes = new[]{
                        _schemeService.GetScheme<IMonsterScheme>("rat"),
                        _schemeService.GetScheme<IMonsterScheme>("rat"),
                        _schemeService.GetScheme<IMonsterScheme>("rat"),
                        _schemeService.GetScheme<IMonsterScheme>("rat"),

                        _schemeService.GetScheme<IMonsterScheme>("rat"),
                        _schemeService.GetScheme<IMonsterScheme>("rat"),
                        _schemeService.GetScheme<IMonsterScheme>("rat"),
                        _schemeService.GetScheme<IMonsterScheme>("rat"),

                        _schemeService.GetScheme<IMonsterScheme>("rat"),
                        _schemeService.GetScheme<IMonsterScheme>("rat"),
                        _schemeService.GetScheme<IMonsterScheme>("rat"),
                        _schemeService.GetScheme<IMonsterScheme>("rat")
                    }
                },

                new MonsterSet{
                    MonsterSchemes = new[]{
                        _schemeService.GetScheme<IMonsterScheme>("rat-human-slayer"),
                        _schemeService.GetScheme<IMonsterScheme>("rat"),
                        _schemeService.GetScheme<IMonsterScheme>("rat"),
                        _schemeService.GetScheme<IMonsterScheme>("rat"),
                        _schemeService.GetScheme<IMonsterScheme>("rat")
                    }
                },

                new MonsterSet{
                    MonsterSchemes = new[]{
                        _schemeService.GetScheme<IMonsterScheme>("genomass"),
                        _schemeService.GetScheme<IMonsterScheme>("gemonass-slave"),
                        _schemeService.GetScheme<IMonsterScheme>("gemonass-slave"),
                        _schemeService.GetScheme<IMonsterScheme>("gemonass-slave"),

                        _schemeService.GetScheme<IMonsterScheme>("genomass"),
                        _schemeService.GetScheme<IMonsterScheme>("gemonass-slave"),
                        _schemeService.GetScheme<IMonsterScheme>("gemonass-slave"),
                        _schemeService.GetScheme<IMonsterScheme>("gemonass-slave")
                    }
                },

                new MonsterSet{
                    MonsterSchemes = new[]{
                        _schemeService.GetScheme<IMonsterScheme>("infernal-bard"),
                        _schemeService.GetScheme<IMonsterScheme>("archidemon")
                    }
                },

                new MonsterSet{
                    MonsterSchemes = new[]{
                        _schemeService.GetScheme<IMonsterScheme>("demon-spearman"),
                        _schemeService.GetScheme<IMonsterScheme>("demon-spearman"),

                        _schemeService.GetScheme<IMonsterScheme>("demon-spearman"),
                        _schemeService.GetScheme<IMonsterScheme>("demon-spearman"),

                        _schemeService.GetScheme<IMonsterScheme>("demon-spearman"),
                        _schemeService.GetScheme<IMonsterScheme>("demon-spearman")
                    }
                },

                new MonsterSet{
                    MonsterSchemes = new[]{
                        _schemeService.GetScheme<IMonsterScheme>("hell-rock")
                    }
                },

                new MonsterSet{
                    MonsterSchemes = new[]{
                        _schemeService.GetScheme<IMonsterScheme>("hell-rock"),
                        _schemeService.GetScheme<IMonsterScheme>("hell-rock")
                    }
                },
            };
        }
    }
}
