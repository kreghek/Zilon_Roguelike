using System.Linq;

using BenchmarkDotNet.Attributes;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Benckmarks.Fow
{
    public class FowDataBench
    {
        private int _radius;
        private SectorHexMap _map;
        private HumanSectorFowData _fowData;
        private HexNode _baseNode;

        [Benchmark(Description = "Calc fow Empty room")]
        public void CalcFow()
        {
            FowHelper.UpdateFowData(_fowData, _map, _baseNode, _radius);
        }

        [IterationSetup]
        public void Setup()
        {
            int mapSize = 1000;
            int baseX = 50;
            int baseY = 50;
            _radius = 5;

            // ARRANGE

            _map = new SectorHexMap(1000);
            for (var i = 0; i < mapSize; i++)
            {
                for (var j = 0; j < mapSize; j++)
                {
                    var hexNode = new HexNode(i, j);
                    _map.AddNode(hexNode);
                }
            }

            _fowData = new HumanSectorFowData();

            _baseNode = _map.HexNodes.Single(x => x.OffsetCoords.X == baseX && x.OffsetCoords.Y == baseY);
        }
    }
}
