using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace Zilon.GlobeObserver
{
    class Program
    {
        static async Task Main()
        {
            var serviceCollection = new ServiceCollection();
            Services.RegisterServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var globeStorage = serviceProvider.GetRequiredService<GlobeStorage>();
            var globeGenerator = serviceProvider.GetRequiredService<IWorldGenerator>();
            var taskSource = serviceProvider.GetRequiredService<IActorTaskSource>();

            var globe = await LoadOrCreateGlobeAsync(globeStorage, globeGenerator);

            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    globe.Iteration++;

                    foreach(var sectorInfo in globe.SectorInfos)
                    {
                        var actorManager = sectorInfo.Sector.ActorManager;

                        var snapshot = new SectorSnapshot(sectorInfo.Sector);

                        NextTurn(actorManager, taskSource, snapshot);

                        sectorInfo.Sector.Update();
                    };

                    Console.WriteLine($"[.] ITERATION {globe.Iteration} PROCESSED");
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            await globeStorage.SaveAsync(globe, "globe");
        }

        private static async Task<Globe> LoadOrCreateGlobeAsync(GlobeStorage globeStorage, IWorldGenerator globeGenerator)
        {
            Globe globe;
            if (!globeStorage.HasFile("globe"))
            {
                var result = await globeGenerator.CreateGlobeAsync().ConfigureAwait(false);
                globe = result.Globe;
            }
            else
            {
                var restoredGlobe = await globeStorage.LoadAsync("globe");
                globe = restoredGlobe;
            }

            return globe;
        }

        private static void NextTurn(IActorManager actors, IActorTaskSource taskSource, SectorSnapshot snapshot)
        {
            foreach (var actor in actors.Items)
            {
                if (actor.Person.CheckIsDead())
                {
                    continue;
                }

                ProcessActor(actor, taskSource, snapshot);
            }
        }

        private static void ProcessActor(IActor actor, IActorTaskSource taskSource, SectorSnapshot snapshot)
        {
            var actorTasks = taskSource.GetActorTasks(actor, snapshot);

            foreach (var actorTask in actorTasks)
            {
                try
                {
                    actorTask.Execute();
                }
                catch (Exception exception)
                {
                    throw new ActorTaskExecutionException($"Ошибка при работе источника команд {taskSource.GetType().FullName}",
                        taskSource,
                        exception);
                }
            }
        }
    }
}
