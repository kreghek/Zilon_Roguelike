using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.ProgressStoring;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace Zilon.GlobeObserver
{
    public class SectorInfoFactory : ISectorInfoFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SectorInfoFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public SectorInfo Create(GlobeRegion globeRegion,
            GlobeRegionNode globeRegionNode,
            SectorStorageData sectorStorageData,
            IEnumerable<ActorStorageData> actors,
            IDictionary<string, IPerson> personDict)
        {
            var scope = _serviceProvider.CreateScope();
            {
                var actorManager = scope.ServiceProvider.GetRequiredService<IActorManager>();
                var propContainerManager = scope.ServiceProvider.GetRequiredService<IPropContainerManager>();
                var taskSource = scope.ServiceProvider.GetRequiredService<IActorTaskSource>();
                var dropResolver = scope.ServiceProvider.GetRequiredService<IDropResolver>();
                var schemeService = scope.ServiceProvider.GetRequiredService<ISchemeService>();
                var equipmentDurableService = scope.ServiceProvider.GetRequiredService<IEquipmentDurableService>();

                var mapStorageData = sectorStorageData.PassMap;
                var map = new SectorHexMap(100);
                foreach (var nodeCoords in mapStorageData)
                {
                    var isObstacle = sectorStorageData.Obstacles.Contains(nodeCoords);
                    var hexNode = new HexNode(nodeCoords.X, nodeCoords.Y, isObstacle);
                    map.AddNode(hexNode);
                }

                var sector = new Sector(map,
                                        actorManager,
                                        propContainerManager,
                                        dropResolver,
                                        schemeService,
                                        equipmentDurableService);

                var sectorManager = scope.ServiceProvider.GetRequiredService<ISectorManager>();
                (sectorManager as GenerationSectorManager).CurrentSector = sector;

                var sectorInfo = new SectorInfo(sector,
                                                globeRegion,
                                                globeRegionNode);

                var regionCounter = 1;
                foreach (var regionCoords in sectorStorageData.Regions)
                {
                    var regionNodes = map.Nodes.Cast<HexNode>()
                        .Where(x => regionCoords.Contains(new Core.OffsetCoords(x.OffsetX, x.OffsetY)))
                        .ToArray();

                    var region = new MapRegion(regionCounter, regionNodes);
                    regionCounter++;

                    sector.Map.Regions.Add(region);
                }

                foreach (var transition in sectorStorageData.Transitions)
                {
                    var transitionNode = sector.Map.Nodes.Cast<HexNode>()
                        .Single(x => x.OffsetX == transition.Coords.X && x.OffsetY == transition.Coords.Y);

                    sector.Map.Transitions.Add(transitionNode, new Core.MapGenerators.RoomTransition(transition.Sid));
                }


                var player = scope.ServiceProvider.GetRequiredService<IBotPlayer>();
                RestoreActors(sectorInfo, personDict, sector, player, actors);

                return sectorInfo;
            }
        }

        private void RestoreActors(SectorInfo sectorInfo,
            IDictionary<string, IPerson> personDict,
            ISector sector,
            IPlayer player,
            IEnumerable<ActorStorageData> actors)
        {
            if (sectorInfo is null)
            {
                throw new ArgumentNullException(nameof(sectorInfo));
            }

            if (personDict is null)
            {
                throw new ArgumentNullException(nameof(personDict));
            }
         
            foreach (var actorStorageData in actors)
            {
                var person = personDict[actorStorageData.PersonId];
                var node = sector.Map.Nodes
                    .Cast<HexNode>()
                    .Single(n => n.OffsetX == actorStorageData.Coords.X && n.OffsetY == actorStorageData.Coords.Y);
                var actor = new Actor(person, player, node);

                sectorInfo.Sector.ActorManager.Add(actor);
            }
        }
    }
}
