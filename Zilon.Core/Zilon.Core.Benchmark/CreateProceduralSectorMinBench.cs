using System.Linq;

using BenchmarkDotNet.Attributes;

using LightInject;

using Moq;

using Zilon.Bot.Players;
using Zilon.Core.Client;
using Zilon.Core.MapGenerators;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.Benchmark
{
    public sealed class CreateProceduralSectorMinBench: CreateSectorBenchBase
    {
        private ServiceContainer _container;

        [Benchmark(Description = "CreateProceduralMinSector")]
        public async System.Threading.Tasks.Task CreateAsync()
        {
            var sectorManager = _container.GetInstance<ISectorManager>();
            var playerState = _container.GetInstance<ISectorUiState>();
            var schemeService = _container.GetInstance<ISchemeService>();
            var humanPlayer = _container.GetInstance<HumanPlayer>();
            var actorManager = _container.GetInstance<IActorManager>();
            var humanActorTaskSource = _container.GetInstance<IHumanActorTaskSource>();

            var locationScheme = new TestLocationScheme
            {
                SectorLevels = new ISectorSubScheme[]
                {
                    new TestSectorSubScheme
                    {
                        RegularMonsterSids = new[] { "rat" },
                        RareMonsterSids = new[] { "rat" },
                        ChampionMonsterSids = new[] { "rat" },

                        RegionCount = 20,
                        RegionSize = 20,

                        IsStart = true,

                        ChestDropTableSids = new[] {"survival", "default" },
                        RegionChestCountRatio = 9,
                        TotalChestCount = 20
                    }
                }
            };

            var globeNode = new GlobeRegionNode(0, 0, locationScheme);
            humanPlayer.GlobeNode = globeNode;

            await sectorManager.CreateSectorAsync();

            var personScheme = schemeService.GetScheme<IPersonScheme>("human-person");

            var playerActorStartNode = sectorManager.CurrentSector.Map.Regions
                .SingleOrDefault(x => x.IsStart).Nodes
                .First();

            var survivalRandomSource = _container.GetInstance<ISurvivalRandomSource>();

            var playerActorVm = BenchHelper.CreateHumanActorVm(humanPlayer,
                schemeService,
                survivalRandomSource,
                personScheme,
                actorManager,
                playerActorStartNode);

            //Лучше централизовать переключение текущего актёра только в playerState
            playerState.ActiveActor = playerActorVm;
            humanActorTaskSource.SwitchActor(playerState.ActiveActor.Actor);

            var gameLoop = _container.GetInstance<IGameLoop>();
            var monsterTaskSource = _container.GetInstance<MonsterBotActorTaskSource>();
            gameLoop.ActorTaskSources = new IActorTaskSource[] {
                humanActorTaskSource,
                monsterTaskSource
            };
        }

        protected override IMonsterGeneratorRandomSource CreateFakeMonsterGeneratorRandomSource()
        {
            var mock = new Mock<IMonsterGeneratorRandomSource>();
            mock.Setup(x => x.RollRegionCount(It.IsAny<int>(), It.IsAny<int>())).Returns(0);
            return mock.Object;
        }

        protected override IChestGeneratorRandomSource CreateFakeChestGeneratorRandomSource()
        {
            var mock = new Mock<IChestGeneratorRandomSource>();
            mock.Setup(x => x.RollChestCount(It.IsAny<int>())).Returns(0);
            return mock.Object;
        }
    }
}
