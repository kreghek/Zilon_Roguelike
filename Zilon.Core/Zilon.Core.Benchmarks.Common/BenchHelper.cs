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
            var schemeLocator = FileSchemeLocator.CreateFromEnvVariable();
            return schemeLocator;
        }

        public static IActorViewModel CreateHumanActorVm([NotNull] IPlayer player,
            [NotNull] ISchemeService schemeService,
            [NotNull] ISurvivalRandomSource survivalRandomSource,
            [NotNull] IPersonScheme personScheme,
            [NotNull] IActorManager actorManager,
            [NotNull] IGraphNode startNode)
        {
            if (player is null)
            {
                throw new System.ArgumentNullException(nameof(player));
            }

            if (schemeService is null)
            {
                throw new System.ArgumentNullException(nameof(schemeService));
            }

            if (survivalRandomSource is null)
            {
                throw new System.ArgumentNullException(nameof(survivalRandomSource));
            }

            if (personScheme is null)
            {
                throw new System.ArgumentNullException(nameof(personScheme));
            }

            if (actorManager is null)
            {
                throw new System.ArgumentNullException(nameof(actorManager));
            }

            if (startNode is null)
            {
                throw new System.ArgumentNullException(nameof(startNode));
            }

            var person = new HumanPerson(personScheme);

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