using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace Assets.Zilon.Scripts.Services
{
    class MySectorGenerator : ISectorGenerator
    {
        private readonly ISectorFactory _sectorFactory;

        public MySectorGenerator(ISectorFactory sectorFactory)
        {
            _sectorFactory = sectorFactory ?? throw new ArgumentNullException(nameof(sectorFactory));
        }

        public async Task<ISector> GenerateAsync(ISectorNode sectorNode)
        {
            var transitions = MapFactoryHelper.CreateTransitions(sectorNode).ToArray();

            var map = await SquareMapFactory.CreateAsync(transitions.Count() + 1);

            CreateTransitions(transitions, map);

            var locationScheme = sectorNode.Biome.LocationScheme;

            var sector = _sectorFactory.Create(map, locationScheme);

            return sector;
        }

        private static void CreateTransitions(RoomTransition[] transitions, ISectorMap map)
        {
            var nodes = map.Nodes.ToArray();
            for (var i = 0; i < transitions.Count(); i++)
            {
                map.Transitions.Add(nodes[i], transitions[i]);
            }
        }
    }
}
