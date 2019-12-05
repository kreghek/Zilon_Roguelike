using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.WildStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
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

        //private static async Task<GenerationResult> CreateGlobeAsync(IServiceProvider serviceProvider)
        //{
        //    var globe = new Globe();

        //    var terrainInitiator = serviceProvider.GetRequiredService<TerrainInitiator>();
        //    var terrain = await terrainInitiator.GenerateAsync();
        //    globe.Terrain = terrain;

        //    const int WORLD_SIZE = 40;
        //    await GenerateAnsAssignRegionsAsync(serviceProvider, globe, WORLD_SIZE);

        //    var scopesList = new ConcurrentBag<IServiceScope>();

        //    // Берём 8 случайных точек. Это стартовые города государсв.
        //    var localityCoords = Enumerable.Range(0, WORLD_SIZE * WORLD_SIZE)
        //        .Take(8)
        //        .OrderBy(x => Guid.NewGuid())
        //        .Select(coordIndex => new Core.OffsetCoords(coordIndex / WORLD_SIZE, coordIndex % WORLD_SIZE));

        //    Parallel.ForEach(globe.Terrain.Regions, async region =>
        //    {
        //        var needToCreateSector = localityCoords.Contains(region.TerrainCell.Coords);

        //        if (needToCreateSector)
        //        {
        //            var regionNode = region.RegionNodes.First();

        //            var scope = serviceProvider.CreateScope();

        //            var mapFactory = scope.ServiceProvider.GetRequiredService<IMapFactory>();
        //            var actorManager = scope.ServiceProvider.GetRequiredService<IActorManager>();
        //            var propContainerManager = scope.ServiceProvider.GetRequiredService<IPropContainerManager>();
        //            var dropResolver = scope.ServiceProvider.GetRequiredService<IDropResolver>();
        //            var schemeService = scope.ServiceProvider.GetRequiredService<ISchemeService>();
        //            var equipmentDurableService = scope.ServiceProvider.GetRequiredService<IEquipmentDurableService>();
        //            var humanPersonFactory = scope.ServiceProvider.GetRequiredService<IHumanPersonFactory>();
        //            var botPlayer = scope.ServiceProvider.GetRequiredService<IBotPlayer>();

        //            var localitySector = await CreateWildSectorAsync(mapFactory,
        //                                                         actorManager,
        //                                                         propContainerManager,
        //                                                         dropResolver,
        //                                                         schemeService,
        //                                                         equipmentDurableService);

        //            var sectorManager = scope.ServiceProvider.GetRequiredService<ISectorManager>();
        //            (sectorManager as GenerationSectorManager).CurrentSector = localitySector;

        //            regionNode.Sector = localitySector;

        //            for (var populationUnitIndex = 0; populationUnitIndex < 4; populationUnitIndex++)
        //            {
        //                for (var personIndex = 0; personIndex < 10; personIndex++)
        //                {
        //                    var node = localitySector.Map.Nodes.ElementAt(personIndex);
        //                    var person = CreatePerson(humanPersonFactory);
        //                    var actor = CreateActor(botPlayer, person, node);
        //                    actorManager.Add(actor);
        //                }
        //            }

        //            scopesList.Add(scope);

        //            var taskSource = scope.ServiceProvider.GetRequiredService<IActorTaskSource>();
        //            var sectorInfo = new SectorInfo(actorManager,
        //                                            propContainerManager,
        //                                            localitySector,
        //                                            region,
        //                                            regionNode,
        //                                            taskSource);
        //            globe.SectorInfos.Add(sectorInfo);
        //        }
        //    });

        //    var result = new GenerationResult(globe);

        //    return result;
        //}

        //private static async Task GenerateAnsAssignRegionsAsync(IServiceProvider serviceProvider, Globe globe, int WORLD_SIZE)
        //{
        //    var provinces = new ConcurrentBag<GlobeRegion>();
        //    for (var terrainCellX = 0; terrainCellX < WORLD_SIZE; terrainCellX++)
        //    {
        //        for (var terrainCellY = 0; terrainCellY < WORLD_SIZE; terrainCellY++)
        //        {
        //            var province = await CreateProvinceAsync(serviceProvider);
        //            province.TerrainCell = new TerrainCell { Coords = new Core.OffsetCoords(terrainCellX, terrainCellY) };
        //            provinces.Add(province);
        //        }
        //    };

        //    globe.Terrain.Regions = provinces.ToArray();
        //}

        private static async Task<GlobeRegion> CreateProvinceAsync(IServiceProvider serviceProvider)
        {
            var provinceInitiator = serviceProvider.GetRequiredService<ProvinceInitiator>();
            var region = await provinceInitiator.GenerateRegionAsync();
            return region;
        }

        private static IPerson CreatePerson(IHumanPersonFactory humanPersonFactory)
        {
            var person = humanPersonFactory.Create();
            return person;
        }

        private static IActor CreateActor(IPlayer personPlayer,
            IPerson person,
            IGraphNode node)
        {
            var actor = new Actor(person, personPlayer, node);

            return actor;
        }

        private static async Task<ISector> CreateWildSectorAsync(IMapFactory mapFactory,
            IActorManager actorManager,
            IPropContainerManager propContainerManager,
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IEquipmentDurableService equipmentDurableService)
        {
            var sectorMap = await WildMapFactory.CreateAsync(100);
            var sector = new Sector(sectorMap,
                                    actorManager,
                                    propContainerManager,
                                    dropResolver,
                                    schemeService,
                                    equipmentDurableService);
            return sector;
        }
    }
}
