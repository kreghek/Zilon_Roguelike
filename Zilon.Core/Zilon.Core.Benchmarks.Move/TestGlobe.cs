using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tests.Common;
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.Benchmarks.Move
{
    internal class TestGlobe : IGlobe
    {
        private readonly TestMaterializedSectorNode _sectorNode;

        public TestGlobe(
            IPersonScheme personScheme,
            IHumanActorTaskSource<ISectorTaskSourceContext> humanActorTaskSource)
        {
            TestSectorSubScheme testSectorSubScheme = new TestSectorSubScheme
            {
                RegularMonsterSids = new[]
                {
                    "rat"
                },
                RegionMonsterCount = 0,
                MapGeneratorOptions =
                    new TestSectorRoomMapFactoryOptionsSubScheme
                    {
                        RegionCount = 20,
                        RegionSize = 20
                    },
                IsStart = true,
                ChestDropTableSids = new[]
                {
                    "survival",
                    "default"
                },
                RegionChestCountRatio = 9,
                TotalChestCount = 0
            };

            _sectorNode = new TestMaterializedSectorNode(testSectorSubScheme);

            var person = new HumanPerson(personScheme, Fractions.MainPersonFraction);

            var playerActorStartNode = _sectorNode.Sector.Map.Regions
                                                  .SingleOrDefault(x => x.IsStart)
                                                  .Nodes
                                                  .First();

            var actor = new Actor(person, humanActorTaskSource, playerActorStartNode);

            _sectorNode.Sector.ActorManager.Add(actor);
        }

        public IEnumerable<ISectorNode> SectorNodes => new[]
        {
            _sectorNode
        };

        public void AddSectorNode(ISectorNode sectorNode)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync()
        {
            throw new NotImplementedException();
        }
    }
}