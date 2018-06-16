using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Zilon.Core.Tactics.Spatial;

//namespace Zilon.Core.Tests.Tactics.Map
//{
//    using System.Collections.Generic;
//    using FluentAssertions;
//    using Moq;
//    using NUnit.Framework;
//    
//    using Zilon.Core.Services.CombatMap;
//    using Zilon.Core.Tactics.Spatial;
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