using System;

using JetBrains.Annotations;

using Zilon.Core.Client;
using Zilon.Core.Graphs;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Benchmark
{
    public static class BenchHelper
    {
        public static ISchemeLocator CreateSchemeLocator()
        {
            var schemePath = Environment.GetEnvironmentVariable("ZILON_LIV_SCHEME_CATALOG");
            var schemeLocator = new FileSchemeLocator(schemePath);
            return schemeLocator;
        }

        public static IActorViewModel CreateHumanActorVm([NotNull] IPlayer player,
            [NotNull] ISchemeService schemeService,
            [NotNull] ISurvivalRandomSource survivalRandomSource,
            [NotNull] IPersonScheme personScheme,
            [NotNull] IActorManager actorManager,
            [NotNull] IGraphNode startNode)
        {
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
