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

namespace Zilon.Emulation.Common
{
    public abstract class AutoplayEngineBase<T> where T : IPluggableActorTaskSource
    {
        private const int ITERATION_LIMIT = 40_000;

        private bool _changeSector;

        protected IServiceScope ServiceScope { get; set; }

        protected BotSettings BotSettings { get; }

        protected AutoplayEngineBase(BotSettings botSettings)
        {
            BotSettings = botSettings;
        }

        public async Task StartAsync(IPerson startPerson, IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var humanActor = await CreateSectorAsync(startPerson, serviceProvider).ConfigureAwait(false);
            var gameLoop = ServiceScope.ServiceProvider.GetRequiredService<IGameLoop>();
            var botActorTaskSource = serviceProvider.GetRequiredService<T>();
            botActorTaskSource.Configure(BotSettings);

            var iterationCounter = 1;
            while (!humanActor.Person.GetModule<ISurvivalModule>().IsDead && iterationCounter <= ITERATION_LIMIT)
            {
                try
                {
                    gameLoop.Update();

                    if (_changeSector)
                    {
                        humanActor = await CreateSectorAsync(startPerson, serviceProvider).ConfigureAwait(false);

                        gameLoop = ServiceScope.ServiceProvider.GetRequiredService<IGameLoop>();
                        botActorTaskSource = ServiceScope.ServiceProvider.GetRequiredService<T>();
                        botActorTaskSource.Configure(BotSettings);

                        _changeSector = false;
                    }
                }
                catch (ActorTaskExecutionException exception)
                {
                    CatchActorTaskExecutionException(exception);
                }
                catch (AggregateException exception)
                {
                    CatchException(exception.InnerException);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    CatchException(exception);
                }
            }

            ProcessEnd();
        }

        protected abstract void CatchException(Exception exception);

        protected abstract void CatchActorTaskExecutionException(ActorTaskExecutionException exception);

        protected abstract void ProcessEnd();

        private static IActor CreateHumanActor(HumanPlayer humanPlayer,
            IPerson humanPerson,
            ISectorManager sectorManager,
            IPlayerEventLogService playerEventLogService)
        {
            var playerActorStartNode = sectorManager.CurrentSector.Map.Regions
                .SingleOrDefault(x => x.IsStart)
                .Nodes
                .First();

            humanPlayer.MainPerson = humanPerson;

            var actor = new Actor(humanPerson, humanPlayer, playerActorStartNode);

            playerEventLogService.Actor = actor;

            sectorManager.CurrentSector.ActorManager.Add(actor);

            return actor;
        }

        private async Task<IActor> CreateSectorAsync(IPerson startPerson, IServiceProvider _globalServiceProvider)
        {
            if (ServiceScope != null)
            {
                ServiceScope.Dispose();
                ServiceScope = null;
            }

            ServiceScope = _globalServiceProvider.CreateScope();

            ConfigBotAux();

            var humanPlayer = _globalServiceProvider.GetRequiredService<HumanPlayer>();
            var scoreManager = _globalServiceProvider.GetRequiredService<IScoreManager>();

            var gameLoop = ServiceScope.ServiceProvider.GetRequiredService<IGameLoop>();
            var sectorManager = ServiceScope.ServiceProvider.GetRequiredService<ISectorManager>();
            var botActorTaskSource = ServiceScope.ServiceProvider.GetRequiredService<T>();
            var monsterActorTaskSource = ServiceScope.ServiceProvider.GetRequiredService<MonsterBotActorTaskSource>();
            var playerEventLogService = ServiceScope.ServiceProvider.GetService<IPlayerEventLogService>();

            await sectorManager.CreateSectorAsync().ConfigureAwait(false);

            sectorManager.CurrentSector.ScoreManager = scoreManager;
            sectorManager.CurrentSector.HumanGroupExit += CurrentSector_HumanGroupExit;

            gameLoop.ActorTaskSources = new Core.Tactics.Behaviour.IActorTaskSource[] {
                botActorTaskSource,
                monsterActorTaskSource
            };

            var humanActor = CreateHumanActor(humanPlayer,
                startPerson,
                sectorManager,
                playerEventLogService);

            return humanActor;
        }

        protected abstract void ConfigBotAux();

        private void CurrentSector_HumanGroupExit(object sender, SectorExitEventArgs e)
        {
            ProcessSectorExit();
            _changeSector = true;

            var sectorManager = ServiceScope.ServiceProvider.GetRequiredService<ISectorManager>();
            sectorManager.CurrentSector.HumanGroupExit -= CurrentSector_HumanGroupExit;

            var humanPlayer = ServiceScope.ServiceProvider.GetRequiredService<HumanPlayer>();
            humanPlayer.BindSectorNode(e.Transition.SectorNode);
        }

        protected abstract void ProcessSectorExit();
    }
}
