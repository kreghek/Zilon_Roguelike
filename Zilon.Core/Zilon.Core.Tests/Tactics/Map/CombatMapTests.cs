namespace Zilon.Core.Tests.Tactics.Map
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Zilon.Core.Math;
    using Zilon.Core.Services.CombatMap;
    using Zilon.Core.Tactics.Map;

    [TestFixture]
    public class CombatMapTests
    {
        /// <summary>
        /// 1. В системе есть дефолный набор нод.
        /// 2. Создаём карту.
        /// 3. Карта успешно создана. Ноды указаны в объекте карты.
        /// </summary>
        [Test]
        public void Constructor_Default_HasNodes()
        {
            var mapGenerator = CreateMapGenerator();
            var map = new CombatMap();
            mapGenerator.CreateMap(map);

            map.Should().NotBeNull();
            map.Nodes.Should().NotBeNull();
            map.TeamNodes.Should().NotBeNull();
        }

        private IMapGenerator CreateMapGenerator()
        {
            var mapGeneratorMock = new Mock<IMapGenerator>();
            mapGeneratorMock
                .Setup(x => x.CreateMap(It.IsAny<ICombatMap>()))
                .Callback<ICombatMap>(CreateTwoNodes);
            return mapGeneratorMock.Object;
        }

        private void CreateTwoNodes(ICombatMap map)
        {

            var nodes = new List<MapNode> {
                new MapNode{
                    Id = 1,
                    Position = new Vector2{X = 1, Y = 1 }
                },
                new MapNode{
                    Id = 1,
                    Position = new Vector2{X = 1, Y = 30 }
                }
            };

            map.Nodes = nodes;
            map.TeamNodes = nodes;
        }
    }
}