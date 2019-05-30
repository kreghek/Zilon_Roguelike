using System;

using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.World
{
    public sealed class WorldManager : IWorldManager
    {
        private readonly ISchemeService _schemeService;
        private readonly IDice _dice;

        public WorldManager(ISchemeService schemeService, IDice dice)
        {
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));

            Regions = new Dictionary<TerrainCell, GlobeRegion>();
        }

        public Globe Globe { get; set; }
        public Dictionary<TerrainCell, GlobeRegion> Regions { get; }

        public void UpdateRegionNodes(GlobeRegion region)
        {
            // Подсчитываем узлы, занятые монстрами.
            // Это делаем для того, чтобы следить за плотностью моснтров в секторе.

            var nodeWithMonsters = region.RegionNodes.Where(x => x.MonsterState != null);

            var monsterLimitIsReached = nodeWithMonsters.Count() >= 100;

            foreach (var node in region.RegionNodes)
            {
                if (node.MonsterState == null)
                {
                    if (!monsterLimitIsReached)
                    {
                        var spawnMonsterRoll = _dice.Roll(6);

                        if (spawnMonsterRoll >= 5)
                        {
                            node.MonsterState = new GlobeRegionNodeMonsterState
                            {
                                MonsterPersons = new MonsterPerson[5]
                            };

                            // Генерируем персонажей монстров.
                            for (var i = 0; i < 5; i++)
                            {
                                var person = new MonsterPerson(_schemeService.GetScheme<IMonsterScheme>("rat"));
                                node.MonsterState.MonsterPersons[i] = person;
                            }
                        }
                    }
                }
            }
        }
    }
}
