using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.ProgressStoring;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.GlobeObserver
{
    public class SectorInfoFactory : ISectorInfoFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SectorInfoFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public SectorInfo Create(GlobeRegion globeRegion, GlobeRegionNode globeRegionNode, SectorStorageData sectorStorageData)
        {
            using (var scope = _serviceProvider.CreateScope())
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

                var sectorInfo = new SectorInfo(actorManager,
                                                propContainerManager,
                                                sector,
                                                globeRegion,
                                                globeRegionNode,
                                                taskSource);

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

                return sectorInfo;
            }
        }
    }
}
