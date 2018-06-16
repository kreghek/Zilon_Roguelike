//using System.Collections.Generic;
//using Moq;
//using Zilon.Core.Math;
//using Zilon.Core.Services.CombatMap;
//using Zilon.Core.Tactics.Map;
//
//namespace Zilon.Core.Tests.TestCommon
//{
//    public static class MapGeneratorMocks
//    {
//        public static IMapGenerator CreateTwoNodesMapGenerator()
//        {
//            var mapGeneratorMock = new Mock<IMapGenerator>();
//            mapGeneratorMock
//                .Setup(x => x.CreateMap(It.IsAny<ICombatMap>()))
//                .Callback<ICombatMap>(CreateTwoNodes);
//            return mapGeneratorMock.Object;
//        }
//
//        private static void CreateTwoNodes(ICombatMap map)
//        {
//
//            var nodes = new List<MapNode> {
//                new MapNode{
//                    Id = 1,
//                    Position = new Vector2{X = 1, Y = 1 }
//                },
//                new MapNode{
//                    Id = 1,
//                    Position = new Vector2{X = 1, Y = 30 }
//                }
//            };
//
//            map.Nodes = nodes;
//            map.TeamNodes = nodes;
//        }
//    }
//}
