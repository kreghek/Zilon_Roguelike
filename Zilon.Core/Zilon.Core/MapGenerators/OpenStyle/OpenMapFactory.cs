using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Common;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.OpenStyle
{
    public sealed class OpenMapFactory : IMapFactory
    {
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

        public async Task<ISectorMap> CreateAsync(ISectorMapFactoryOptions generationOptions)
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
            var mapWidth = (mapSize * 2) + 2;
            var mapHeight = mapWidth;

            return await Task.Run(() =>
            {
                ISectorMap map = new SectorHexMap(mapWidth);

                var centerNode = new HexNode(mapSize, mapSize);
                map.AddNode(centerNode);

                FillMap(map, mapWidth, mapHeight, mapSize, centerNode);

                map.Regions.Add(new MapRegion(1, map.Nodes.ToArray()));

                return map;
            });
        }
    }
}