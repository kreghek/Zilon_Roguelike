using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.OpenStyle
{
    public sealed class OpenMapFactory : IMapFactory
    {
        private const int START_REGION_ID = 1;
        private readonly IDice _dice;

        public OpenMapFactory(IDice dice)
        {
            _dice = dice;
        }

        public async Task<ISectorMap> CreateInternalAsync(int mapSize, IEnumerable<SectorTransition> transitions)
        {
            var mapWidth = (mapSize * 2) + 2;
            var mapHeight = mapWidth;

            return await Task.Run(() =>
            {
                ISectorMap map = new SectorHexMap(mapWidth);

                var centerNode = new HexNode(mapSize, mapSize);
                map.AddNode(centerNode);

                FillMap(map, mapWidth, mapHeight, mapSize, centerNode);

                CreateRegionsAndTranstions(map, mapSize, centerNode, transitions);

                map.Regions.Add(new MapRegion(1, map.Nodes.ToArray()));

                return map;
            });
        }

        private void CreateRegionsAndTranstions(ISectorMap map, int mapSize, HexNode centerNode,
            IEnumerable<SectorTransition> transitions)
        {
            var availableNodes = map.Nodes.OfType<HexNode>().ToList();

            // start region

            var startX = _dice.Roll(mapSize / 2) + centerNode.OffsetCoords.X;
            var startY = _dice.Roll(mapSize / 2) + centerNode.OffsetCoords.Y;

            var startRegionNodes = new[] { availableNodes.Single(x => x.OffsetCoords.CompsEqual(startX, startY)) };
            var startRegion = new MapRegion(START_REGION_ID, startRegionNodes)
            {
                IsStart = true
            };
            map.Regions.Add(startRegion);
            foreach (var node in startRegionNodes)
            {
                availableNodes.Remove(node);
            }

            // transition regions

            var transitionsList = transitions.ToArray();
            for (var i = 0; i < transitionsList.Length; i++)
            {
                var roundPart = Math.PI * 2 * i / transitionsList.Length;
                var x = (int)(Math.Cos(roundPart) * mapSize) + centerNode.OffsetCoords.X;
                var y = (int)(Math.Sin(roundPart) * mapSize) + centerNode.OffsetCoords.Y;

                var transitionRegionNodes = new[] { availableNodes.Single(node => node.OffsetCoords.CompsEqual(x, y)) };
                var transitionRegionId = START_REGION_ID + i + 1;
                var transitionRegion = new MapRegion(transitionRegionId, transitionRegionNodes);
                transitionRegion.ExitNodes = transitionRegion.Nodes;
                map.Regions.Add(transitionRegion);

                foreach (var node in transitionRegionNodes)
                {
                    availableNodes.Remove(node);
                    map.Transitions[node] = transitionsList[i];
                }
            }

            // main region
            var mainRegion = new MapRegion(1000, availableNodes.ToArray());
            map.Regions.Add(mainRegion);
        }

        private static void FillMap(ISectorMap map, int mapWidth, int mapHeight, int mapSize, HexNode centerNode)
        {
            for (var x = 0; x < mapWidth; x++)
            {
                for (var y = 0; y < mapHeight; y++)
                {
                    var testOffsetCoords = new OffsetCoords(x, y);
                    var testCubeCoords = HexHelper.ConvertToCube(testOffsetCoords);
                    var distanceToCenter = centerNode.CubeCoords.DistanceTo(testCubeCoords);
                    if (distanceToCenter > 0 && distanceToCenter <= mapSize)
                    {
                        var node = new HexNode(testOffsetCoords);
                        map.AddNode(node);
                    }
                }
            }
        }

        public Task<ISectorMap> CreateAsync(ISectorMapFactoryOptions generationOptions)
        {
            if (generationOptions is null)
            {
                throw new ArgumentNullException(nameof(generationOptions));
            }

            var factoryOptions = (ISectorOpenMapFactoryOptionsSubScheme)generationOptions.OptionsSubScheme;
            if (factoryOptions == null)
            {
                throw new ArgumentException(
                    $"Map generation options {nameof(ISectorSubScheme.MapGeneratorOptions)} are not defined in {nameof(generationOptions)}.");
            }

            var mapSize = factoryOptions.Size;

            return CreateInternalAsync(mapSize, generationOptions.Transitions);
        }
    }
}