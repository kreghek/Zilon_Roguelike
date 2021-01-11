using System.Collections.Generic;

using Moq;

using NUnit.Framework;

using Zilon.Bot.Players.Tests.Triggers;
using Zilon.Core.Graphs;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Triggers.Tests
{
    [TestFixture]
    public class ExploredTriggerTests
    {
        /// <summary>
        /// Test checks trigger for persons with fow module.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(ExploredTriggerTestCaseSource), nameof(ExploredTriggerTestCaseSource.FowModuleTestCases))]
        public bool Test_PersonHasFowDataModule_ReturnsExpectedResults(IEnumerable<IGraphNode> mapNodes, IEnumerable<SectorMapFowNode> fowNodes)
        {
            // ARRANGE

            var trigger = new ExploredTrigger();

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

            return fact;
        }
    }
}