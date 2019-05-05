using System.Threading.Tasks;
using LightInject;
using Zilon.Core.Client;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var container = new ServiceContainer();
            var startUp = new Startup();

            startUp.ConfigureServices(container);

            var gameLoop = container.GetInstance<IGameLoop>();
            var sectorManager = container.GetInstance<ISectorManager>();
            var scoreManager = container.GetInstance<IScoreManager>();
            var humanActorTaskSource = container.GetInstance<IActorTaskSource>("bot");
            var monsterActorTaskSource = container.GetInstance<IActorTaskSource>("monster");

            await sectorManager.CreateSectorAsync();

            sectorManager.CurrentSector.ScoreManager = scoreManager;

            gameLoop.ActorTaskSources = new[] {
                humanActorTaskSource,
                monsterActorTaskSource
            };
        }
    }
}
