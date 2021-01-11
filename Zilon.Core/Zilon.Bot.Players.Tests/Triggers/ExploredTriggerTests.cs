using NUnit.Framework;
using Zilon.Bot.Players.Triggers;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using FluentAssertions;
using Zilon.Core.Persons;
using Zilon.Core.PersonModules;
using Zilon.Core.Graphs;
using System.Linq;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Triggers.Tests
{
    [TestFixture]
    public class ExploredTriggerTests
    {
        /// <summary>
        /// Test checks if all nodes of map are known by fow module then the trigger returns true.
        /// </summary>
        [Test]
        public void Test_AllNodesExploredInFowModule_ReturnsTrue()
        {
            // ARRANGE

            var trigger = new ExploredTrigger();

            var mapNodes = new[] { Mock.Of<IGraphNode>() };

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;
            actorMock.SetupGet(x => x.Person).Returns(person);

            var fowDataMock = new Mock<IFowData>();
            var fowData = fowDataMock.Object;
            personMock.Setup(x => x.GetModule<IFowData>(nameof(IFowData))).Returns(fowData);
            personMock.Setup(x => x.HasModule(nameof(IFowData))).Returns(true);

            var sectorFowDataMock = new Mock<ISectorFowData>();
            var sectorFowData = sectorFowDataMock.Object;
            fowDataMock.Setup(x => x.GetSectorFowData(It.IsAny<ISector>())).Returns(sectorFowData);
            var fowNodes = mapNodes.Select(node=>new SectorMapFowNode(node));
            sectorFowDataMock.Setup(x => x.GetFowNodeByState(It.Is<SectorMapNodeFowState>(state => state == SectorMapNodeFowState.Explored))).Returns(fowNodes);

            var context = Mock.Of<ISectorTaskSourceContext>(c => c.Sector == Mock.Of<ISector>(sector => 
                sector.Map == Mock.Of<ISectorMap>(map => 
                    map.Nodes == mapNodes
                )
            ));

            var state = Mock.Of<ILogicState>();

            var data = Mock.Of<ILogicStrategyData>();

            // ACT

            var fact = trigger.Test(actor, context, state, data);

            // ASSERT

            fact.Should().BeTrue();
        }

        /// <summary>
        /// Test checks if not all nodes of map are known by fow module then the trigger returns false (there are nodes of map which not explored yet).
        /// </summary>
        [Test]
        public void Test_NotAllNodesExploredInFowModule_ReturnsFalse()
        {
            // ARRANGE

            var trigger = new ExploredTrigger();

            var mapNodes = new[] { Mock.Of<IGraphNode>(), Mock.Of<IGraphNode>() };

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;
            actorMock.SetupGet(x => x.Person).Returns(person);

            var fowDataMock = new Mock<IFowData>();
            var fowData = fowDataMock.Object;
            personMock.Setup(x => x.GetModule<IFowData>(nameof(IFowData))).Returns(fowData);
            personMock.Setup(x => x.HasModule(nameof(IFowData))).Returns(true);

            var sectorFowDataMock = new Mock<ISectorFowData>();
            var sectorFowData = sectorFowDataMock.Object;
            fowDataMock.Setup(x => x.GetSectorFowData(It.IsAny<ISector>())).Returns(sectorFowData);
            var fowNodes = mapNodes.Skip(1).Select(node => new SectorMapFowNode(node));
            sectorFowDataMock.Setup(x => x.GetFowNodeByState(It.Is<SectorMapNodeFowState>(state => state == SectorMapNodeFowState.Explored))).Returns(fowNodes);

            var context = Mock.Of<ISectorTaskSourceContext>(c => c.Sector == Mock.Of<ISector>(sector =>
                sector.Map == Mock.Of<ISectorMap>(map =>
                    map.Nodes == mapNodes
                )
            ));

            var state = Mock.Of<ILogicState>();

            var data = Mock.Of<ILogicStrategyData>();

            // ACT

            var fact = trigger.Test(actor, context, state, data);

            // ASSERT

            fact.Should().BeFalse();
        }
    }
}