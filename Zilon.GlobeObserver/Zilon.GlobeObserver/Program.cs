using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.MapGenerators.WildStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.GlobeObserver
{
    class Program
    {
        static async Task Main()
        {
            var serviceCollection = new ServiceCollection();
            Services.RegisterServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var scopes = await CreateGlobeAsync(serviceProvider);

            var iteraion = 0;

            while (iteraion < 40000)
            {
                Parallel.ForEach(scopes, scope =>
                {
                    var taskSource = scope.ServiceProvider.GetRequiredService<IActorTaskSource>();
                    var actorManager = scope.ServiceProvider.GetRequiredService<IActorManager>();
                    NextTurn(actorManager, taskSource);
                });

                iteraion++;
            }
        }


        private static void NextTurn(IActorManager actors, IActorTaskSource taskSource)
        {
            foreach (var actor in actors.Items)
            {
                if (actor.Person.CheckIsDead())
                {
                    continue;
                }

                ProcessActor(actor, taskSource);
            }
        }

        private static void ProcessActor(IActor actor, IActorTaskSource taskSource)
        {
            var actorTasks = taskSource.GetActorTasks(actor);

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

        private static async Task<IList<IServiceScope>> CreateGlobeAsync(IServiceProvider serviceProvider)
        {
            var globeState = new GlobeState();

            var terrainInitiator = serviceProvider.GetRequiredService<TerrainInitiator>();
            var terrain = await terrainInitiator.GenerateAsync();
            globeState.Terrain = terrain;

            var localityCoords = Enumerable.Range(0, 1600).Take(16).OrderBy(x => Guid.NewGuid()).Select(x => new Core.OffsetCoords(x / 40, x % 40));

            var provinces = new ConcurrentBag<GlobeRegion>();
            for (var terrainCellX = 0; terrainCellX < 40; terrainCellX++)
            {
                for (var terrainCellY = 0; terrainCellY < 40; terrainCellY++)
                {
                    var province = await CreateProvinceAsync(serviceProvider);
                    province.TerrainCell = new TerrainCell { Coords = new Core.OffsetCoords(terrainCellX, terrainCellY) };
                    provinces.Add(province);
                }
            };

            var scopesList = new ConcurrentBag<IServiceScope>();

            Parallel.ForEach(provinces, async province =>
            {
                var needToCreateSector = localityCoords.Contains(province.TerrainCell.Coords);

                if (needToCreateSector)
                {
                    var provinceNode = province.RegionNodes.First();

                    var scope = serviceProvider.CreateScope();

                    var mapFactory = scope.ServiceProvider.GetRequiredService<IMapFactory>();
                    var actorManager = scope.ServiceProvider.GetRequiredService<IActorManager>();
                    var propContainerManager = scope.ServiceProvider.GetRequiredService<IPropContainerManager>();
                    var dropResolver = scope.ServiceProvider.GetRequiredService<IDropResolver>();
                    var schemeService = scope.ServiceProvider.GetRequiredService<ISchemeService>();
                    var equipmentDurableService = scope.ServiceProvider.GetRequiredService<IEquipmentDurableService>();
                    var humanPersonFactory = scope.ServiceProvider.GetRequiredService<IHumanPersonFactory>();
                    var botPlayer = scope.ServiceProvider.GetRequiredService<IBotPlayer>();

                    var localitySector = await CreateWildSectorAsync(mapFactory,
                                                                 actorManager,
                                                                 propContainerManager,
                                                                 dropResolver,
                                                                 schemeService,
                                                                 equipmentDurableService);

                    var sectorManager = scope.ServiceProvider.GetRequiredService<ISectorManager>();
                    (sectorManager as GenerationSectorManager).CurrentSector = localitySector;

                    provinceNode.Sector = localitySector;

                    for (var populationUnitIndex = 0; populationUnitIndex < 4; populationUnitIndex++)
                    {
                        for (var personIndex = 0; personIndex < 10; personIndex++)
                        {
                            var node = localitySector.Map.Nodes.ElementAt(personIndex);
                            var person = CreatePerson(humanPersonFactory);
                            var actor = CreateActor(botPlayer, person, node);
                            actorManager.Add(actor);
                        }
                    }

                    scopesList.Add(scope);
                }
            });

            //Parallel.For(0, 300, async localityIndex =>
            //{
            //    var scope = serviceProvider.CreateScope();

            //    var mapFactory = scope.ServiceProvider.GetRequiredService<IMapFactory>();
            //    var actorManager = scope.ServiceProvider.GetRequiredService<IActorManager>();
            //    var propContainerManager = scope.ServiceProvider.GetRequiredService<IPropContainerManager>();
            //    var dropResolver = scope.ServiceProvider.GetRequiredService<IDropResolver>();
            //    var schemeService = scope.ServiceProvider.GetRequiredService<ISchemeService>();
            //    var equipmentDurableService = scope.ServiceProvider.GetRequiredService<IEquipmentDurableService>();
            //    var humanPersonFactory = scope.ServiceProvider.GetRequiredService<IHumanPersonFactory>();
            //    var botPlayer = scope.ServiceProvider.GetRequiredService<IBotPlayer>();

            //    var localitySector = await CreateLocalitySectorAsync(mapFactory,
            //                                                         actorManager,
            //                                                         propContainerManager,
            //                                                         dropResolver,
            //                                                         schemeService,
            //                                                         equipmentDurableService);

            //    var sectorManager = scope.ServiceProvider.GetRequiredService<ISectorManager>();
            //    (sectorManager as GenerationSectorManager).CurrentSector = localitySector;

            //    for (var populationUnitIndex = 0; populationUnitIndex < 4; populationUnitIndex++)
            //    {
            //        for (var personIndex = 0; personIndex < 10; personIndex++)
            //        {
            //            var node = localitySector.Map.Nodes.ElementAt(personIndex);
            //            var person = CreatePerson(humanPersonFactory);
            //            var actor = CreateActor(botPlayer, person, node);
            //            actorManager.Add(actor);
            //        }
            //    }

            //    scopesList.Add(scope);
            //});

            return scopesList.ToList();
        }

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
            IMapNode node)
        {
            var actor = new Actor(person, personPlayer, node);

            return actor;
        }

        private static async Task<ISector> CreateLocalitySectorAsync(IMapFactory mapFactory,
            IActorManager actorManager,
            IPropContainerManager propContainerManager,
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IEquipmentDurableService equipmentDurableService)
        {
            var townScheme = new TownSectorScheme
            {
                RegionCount = 10,
                RegionSize = 10
            };

            var sectorMap = await mapFactory.CreateAsync(townScheme);
            var sector = new Sector(sectorMap,
                                    actorManager,
                                    propContainerManager,
                                    dropResolver,
                                    schemeService,
                                    equipmentDurableService);
            return sector;
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
