using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;
using Zilon.Bot.Sdk;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace Zilon.Emulation.Common
{
    public abstract class AutoplayEngineBase<T> where T : IPluggableActorTaskSource<ISectorTaskSourceContext>
    {
        private const int ITERATION_LIMIT = 40_000_000;

        protected IServiceScope ServiceScope { get; set; }

        protected BotSettings BotSettings { get; }

        protected AutoplayEngineBase(BotSettings botSettings)
        {
            BotSettings = botSettings;
        }

        public async Task StartAsync(IPerson humanPerson, IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            // Create globe
            var globeInitializer = serviceProvider.GetRequiredService<IGlobeInitializer>();
            var globe = await globeInitializer.CreateGlobeAsync("intro").ConfigureAwait(false);

            var player = serviceProvider.GetRequiredService<IPlayer>();
            var followedPerson = player.MainPerson;

            var iterationCounter = 1;
            while (!followedPerson.GetModule<ISurvivalModule>().IsDead && iterationCounter <= ITERATION_LIMIT)
            {
                try
                {
                    await globe.UpdateAsync().ConfigureAwait(false);
                }
                catch (ActorTaskExecutionException exception)
                {
                    CatchActorTaskExecutionException(exception);
                }
                catch (AggregateException exception)
                {
                    CatchException(exception.InnerException);
                    throw;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    CatchException(exception);
                    throw;
                }
            }

            ProcessEnd();
        }

        protected abstract void CatchException(Exception exception);

        protected abstract void CatchActorTaskExecutionException(ActorTaskExecutionException exception);

        protected abstract void ProcessEnd();

        protected abstract void ConfigBotAux();

        protected abstract void ProcessSectorExit();
    }
}
