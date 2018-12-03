using System;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.MapGenerators
{
    [TestFixture]
    public class DungeonMapFactoryTests
    {
        /// <summary>
        /// Тест проверяет, что карта из цепочки комнат строится без ошибок.
        /// </summary>
        [Test]
        public void Create_SimpleSnakeMaze_NoExceptions()
        {
            var randomSource = new TestSnakeRandomSource();


            var factory = new DungeonMapFactory(randomSource);



            // ACT
            Action act = () =>
            {
                var map = factory.Create();
            };



            // ARRANGE
            act.Should().NotThrow();
        }

        /// <summary>
        /// Тест проверяет, что произвольная карта строится без ошибок. И за допустимое время.
        /// </summary>
        [Test]
        [Timeout(3 * 60 * 1000)]
        public void Create_RealRandom_NoExceptions()
        {
            // ARRANGE
            var dice = new Dice(123);
            var randomSource = new SectorGeneratorRandomSource(dice);
            var factory = new DungeonMapFactory(randomSource);



            // ACT
            Action act = () =>
            {
                var map = factory.Create();
            };



            // ARRANGE
            act.Should().NotThrow();
        }

        /// <summary>
        /// Тест проверяет, что карта из цепочки комнат строится без ошибок.
        /// </summary>
        [Test]
        public void Create_RealRandom_NoOverlapNodes()
        {
            // ARRANGE
            var dice = new Dice(3245);
            var randomSource = new SectorGeneratorRandomSource(dice);
            var factory = new DungeonMapFactory(randomSource);



            // ACT
            var map = factory.Create();



            // ARRANGE
            var hexNodes = map.Nodes.Cast<HexNode>().ToArray();
            foreach (var node in hexNodes)
            {
                var sameNode = hexNodes.Where(x => x != node && x.OffsetX == node.OffsetX && x.OffsetY == node.OffsetY);
                sameNode.Should().BeEmpty();
            }
        }

        /// <summary>
        /// Тест проверяет, генератор не создаёт одинаковых ребер (равные соединённые узлы).
        /// </summary>
        [Test]
        public void Create_RealRandom_NoOverlapEdges()
        {
            // ARRANGE
            var dice = new Dice(3245);
            var randomSource = new SectorGeneratorRandomSource(dice);
            var factory = new DungeonMapFactory(randomSource);



            // ACT
            var map = factory.Create();



            // ARRANGE
            foreach (var edge in map.Edges)
            {
                var sameEdge = map.Edges.Where(x => x != edge &&
                    (
                        (x.Nodes[0] == edge.Nodes[0] && x.Nodes[1] == edge.Nodes[1]) ||
                        (x.Nodes[0] == edge.Nodes[1] && x.Nodes[1] == edge.Nodes[0])
                    )
                );
                sameEdge.Should().BeEmpty($"Ребро с {edge.Nodes[0]} и {edge.Nodes[1]} уже есть.");
            }
        }

        private static IMapFactory CreateFactory(ISectorGeneratorRandomSource randomSource)
        {
            return new DungeonMapFactory(randomSource);
        }
    }
}