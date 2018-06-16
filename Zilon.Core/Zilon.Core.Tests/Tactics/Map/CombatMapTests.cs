using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Zilon.Core.Tactics.Map;

namespace Zilon.Core.Tactics.Map.Tests
{
    [TestFixture()]
    public class CombatMapTests
    {
        [Test()]
        public void GetNeiberhoodsTest()
        {
            // ARRANGE

            var map = new CombatMap();
            map.Nodes = new List<MapNode>();

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    map.Nodes.Add(new MapNode() { Coordinates = new Math.Vector2(i, j) });
                }
            }

            var testedNode = map.Nodes.SingleOrDefault(n => n.Coordinates.X == 3 && n.Coordinates.Y == 3);

            var expectedNeibours = new[] {
                map.Nodes.SingleOrDefault(n => n.Coordinates.X == 3 && n.Coordinates.Y == 2),
                map.Nodes.SingleOrDefault(n => n.Coordinates.X == 4 && n.Coordinates.Y == 2),

                map.Nodes.SingleOrDefault(n => n.Coordinates.X == 2 && n.Coordinates.Y == 3),
                map.Nodes.SingleOrDefault(n => n.Coordinates.X == 4 && n.Coordinates.Y == 3),

                map.Nodes.SingleOrDefault(n => n.Coordinates.X == 3 && n.Coordinates.Y == 4),
                map.Nodes.SingleOrDefault(n => n.Coordinates.X == 4 && n.Coordinates.Y == 4)
            };


            // ACT
            var factNeibours = map.GetNeiberhoods(testedNode);



            // ASSERT
            factNeibours.Should().BeEquivalentTo(expectedNeibours);
        }
    }
}
//namespace Zilon.Core.Tests.Tactics.Map
//{
//    using System.Collections.Generic;
//    using FluentAssertions;
//    using Moq;
//    using NUnit.Framework;
//    using Zilon.Core.Math;
//    using Zilon.Core.Services.CombatMap;
//    using Zilon.Core.Tactics.Map;
//    using Zilon.Core.Tests.TestCommon;
//
//    [TestFixture]
//    public class CombatMapTests
//    {
//        /// <summary>
//        /// 1. В системе есть дефолный набор нод.
//        /// 2. Создаём карту.
//        /// 3. Карта успешно создана. Ноды указаны в объекте карты.
//        /// </summary>
//        [Test]
//        public void Constructor_Default_HasNodes()
//        {
//            var mapGenerator = MapGeneratorMocks.CreateTwoNodesMapGenerator();
//            var map = new CombatMap();
//            mapGenerator.CreateMap(map);
//
//            map.Should().NotBeNull();
//            map.Nodes.Should().NotBeNull();
//            map.TeamNodes.Should().NotBeNull();
//        }
//    }
//}