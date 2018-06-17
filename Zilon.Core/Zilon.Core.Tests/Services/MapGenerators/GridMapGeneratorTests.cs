using NUnit.Framework;
using System.Linq;
using Zilon.Core.Tests.TestCommon;
using Zilon.Core.Tactics.Spatial;
using FluentAssertions;
using System.Collections.Generic;
using Moq;
using System;

namespace Zilon.Core.Services.MapGenerators.Tests
{
    [TestFixture()]
    public class GridMapGeneratorTests
    {
        [Test()]
        public void CreateMap_FixedMap_EdgesAreCorrect()
        {
            // ARRANGE
            var nodes = new List<HexNode>();
            var edges = new List<Edge>();

            var mapMock = new Mock<IHexMap>();
            mapMock.SetupProperty(x => x.Nodes, nodes);
            mapMock.SetupProperty(x => x.Edges, edges);
            var map = mapMock.Object;

            var mapGenerator = new GridMapGenerator(7);


            // ACT
            mapGenerator.CreateMap(map);



            // ASSERT
            AssertEdge(map, 0, 0, 1, 0);
            AssertEdge(map, 3, 1, 4, 2);

            // полный набор соседей
            AssertEdge(map, 1, 1, 0, 1);
            AssertEdge(map, 1, 1, 1, 0);
            AssertEdge(map, 1, 1, 2, 0);
            AssertEdge(map, 1, 1, 2, 1);
            AssertEdge(map, 1, 1, 2, 2);
            AssertEdge(map, 1, 1, 1, 2);

            // полный набор соседей для углового
            AssertEdge(map, 6, 6, 5, 6);
            AssertEdge(map, 6, 6, 5, 5);
            AssertEdge(map, 6, 6, 6, 5);

        }

        /// <summary>
        /// Тест проверяет, что генератор сеточных карт может работать с <see cref="HexMap"/>.
        /// <see cref="HexMap"/> используется на клиенте.
        /// </summary>
        [Test()]
        public void CreateMap_HexMapType_NoExceptions()
        {
            // ARRANGE
            
            var map = new HexMap();

            var mapGenerator = new GridMapGenerator(7);


            // ACT
            Action act = () => { mapGenerator.CreateMap(map); };



            // ASSERT
            act.Should().NotThrow();
        }

        private static Edge GetExistsEdge(IHexMap map, HexNode node, HexNode neighbor)
        {
            return (from edge in map.Edges
                    where edge.Nodes.Contains(node)
                    where edge.Nodes.Contains(neighbor)
                    select edge).SingleOrDefault();
        }

        private void AssertEdge(IHexMap map, int offsetX1, int offsetY1, int offsetX2, int offsetY2)
        {
            var node1 = map.Nodes.SelectBy(offsetX1, offsetY1);
            var node2 = map.Nodes.SelectBy(offsetX2, offsetY2);
            var edge = GetExistsEdge(map, node1, node2);
            edge.Should().NotBeNull();
        }
    }
}