using System.Linq;

using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Common
{
    public class SquareMapFactory : IMapFactory
    {
        private readonly int _mapSize;

        public SquareMapFactory(int mapSize)
        {
            _mapSize = mapSize;
        }

        public IMap Create()
        {
            var map = new HexMap();
            MapFiller.FillSquareMap(map, _mapSize);
            map.StartNodes = map.Nodes.Take(1).ToArray();
            map.ExitNodes = new[] { map.Nodes.Last() };
            return map;
        }

        public static IMap Create(int mapSize)
        {
            var factory = new SquareMapFactory(mapSize);
            return factory.Create();
        }
    }
}
