using BenchmarkDotNet.Attributes;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Client;
using Zilon.Core.Graphs;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Benchmarks.CreateSector
{
    public class CreateCellularAutomatonSectorBench
    {
        private ServiceProvider _serviceProvider;

        [Benchmark(Description = "Create CA Sector")]
        public async System.Threading.Tasks.Task CreateSectorAsync()
        {
            var sectorManager = _serviceProvider.GetRequiredService<ISectorManager>();

            await sectorManager.CreateSectorAsync().ConfigureAwait(false);
        }


        [IterationSetup]
        public void IterationSetup()
        {
            var startUp = new Startup();
            var serviceCollection = new ServiceCollection();
            startUp.RegisterServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private IActorViewModel CreateHumanActorVm([NotNull] IPlayer player,
        [NotNull] IPersonScheme personScheme,
        [NotNull] IActorManager actorManager,
        [NotNull] IGraphNode startNode)
        {
            var schemeService = _serviceProvider.GetRequiredService<ISchemeService>();
            var survivalRandomSource = _serviceProvider.GetRequiredService<ISurvivalRandomSource>();

            var inventory = new Inventory();

            var evolutionData = new EvolutionData(schemeService);

            var defaultActScheme = schemeService.GetScheme<ITacticalActScheme>(personScheme.DefaultAct);

            var person = new HumanPerson(personScheme,
                defaultActScheme,
                evolutionData,
                survivalRandomSource,
                inventory);

            var actor = new Actor(person, player, startNode);

            actorManager.Add(actor);

            var actorViewModel = new TestActorViewModel
            {
                Actor = actor
            };

            return actorViewModel;
        }
    }
}
