﻿using System.Linq;

using BenchmarkDotNet.Attributes;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Benchmarks.Fow
{
    [MemoryDiagnoser]
    public class FowDataBench
    {
        private HexNode _baseNode;
        private TestFowContext _fowContextMock;
        private HumanSectorFowData _fowData;
        private int _radius;

        [Benchmark(Description = "Calc fow Empty room")]
        public void CalcFow()
        {
            FowHelper.UpdateFowData(_fowData, _fowContextMock, _baseNode, _radius);
        }

        [IterationSetup]
        public void Setup()
        {
            var mapSize = 1000;
            var baseX = 50;
            var baseY = 50;
            _radius = 5;

            // ARRANGE

            var map = new SectorHexMap(1000);
            for (var i = 0; i < mapSize; i++)
            {
                for (var j = 0; j < mapSize; j++)
                {
                    var hexNode = new HexNode(i, j);
                    map.AddNode(hexNode);
                }
            }

            _fowData = new HumanSectorFowData();

            _baseNode = map.HexNodes.Single(x => x.OffsetCoords.X == baseX && x.OffsetCoords.Y == baseY);

            _fowContextMock = new TestFowContext(map);
        }
    }
}