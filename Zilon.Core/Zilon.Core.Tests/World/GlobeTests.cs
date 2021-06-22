using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace Zilon.Core.Tests.World
{
    [TestFixture]
    public class GlobeTests
    {
        /// <summary>
        /// The test checks complext logic:
        /// - Globe contains one node with materialized sector.
        /// - There is a actor in the sector.
        /// - The actor tries to transit in other mock sector via transi task.
        /// - The test checks what the actor is moves to other sector then the ask is complete.
        /// </summary>
        [Test]
        [Timeout(5000)]
        public async Task UpdateAsyncTest_SectorFiresTransition_GlobeTransitionHandlersInitiatesTransitions()
        {
            // ARRANGE

            var transitionHandlerMock = new Mock<IGlobeTransitionHandler>();
            var transitionHandler = transitionHandlerMock.Object;

            var globe = new Globe(transitionHandler);

            var sectorNodeMock = new Mock<ISectorNode>();
            sectorNodeMock.SetupGet(x => x.State).Returns(SectorNodeState.SectorMaterialized);
            var sectorNode = sectorNodeMock.Object;

            var transitionNode = Mock.Of<IGraphNode>();

            var sectorMock = new Mock<ISector>();
            sectorMock.SetupGet(x => x.Map)
                .Returns(Mock.Of<ISectorMap>(map =>
                    map.Transitions == new Dictionary<IGraphNode, SectorTransition>
                    {
                        { transitionNode, new SectorTransition(Mock.Of<ISectorNode>()) }
                    }
                ));
            var sector = sectorMock.Object;
            sectorNodeMock.SetupGet(x => x.Sector).Returns(sector);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Person).Returns(Mock.Of<IPerson>(person => person.Id == 0));
            actorMock.SetupGet(x => x.CanExecuteTasks).Returns(true);
            actorMock.SetupGet(x => x.Node).Returns(transitionNode);
            var actor = actorMock.Object;

            var taskSourceMock = new Mock<IActorTaskSource<ISectorTaskSourceContext>>();
            var taskSource = taskSourceMock.Object;
            var taskSourceContext = Mock.Of<ISectorTaskSourceContext>(context => context.Sector == sector);
            var taskContext = Mock.Of<IActorTaskContext>(taskContext => taskContext.Sector == taskSourceContext.Sector);
            taskSourceMock.Setup(x => x.GetActorTaskAsync(It.IsAny<IActor>(), It.IsAny<ISectorTaskSourceContext>()))
                .Returns<IActor, ISectorTaskSourceContext>((actor, context) =>
                    Task.FromResult((IActorTask)new SectorTransitTask(actor, taskContext)));

            actorMock.SetupGet(x => x.TaskSource).Returns(taskSource);

            var actorManagerMock = new Mock<IActorManager>();
            var actorManager = actorManagerMock.Object;
            actorManagerMock.SetupGet(x => x.Items).Returns(new[] { actor });
            sectorMock.SetupGet(x => x.ActorManager).Returns(actorManager);

            globe.AddSectorNode(sectorNode);

            // ACT

            for (var i = 0; i < GlobeMetrics.OneIterationLength; i++)
            {
                await globe.UpdateAsync(CancellationToken.None).ConfigureAwait(false);
            }

            // ASSERT

            actorMock.Verify(x => x.MoveToOtherSector(It.Is<ISector>(s => s == sector), It.IsAny<SectorTransition>()),
                Times.Once);
        }

        /// <summary>
        /// The test checks invocation of globe transition handler. Every completed iteration the globe must call UpdateAsync of
        /// the globe transition handler.
        /// </summary>
        /// <param name="globeUpdatesCount"> Count of the globe's update calls. </param>
        /// <param name="expectedHandlersUpdateCount">
        /// Expected count of update invocation of the globe transition handler's
        /// update.
        /// </param>
        /// <returns></returns>
        [Test]
        [Timeout(5000)]
        [TestCase(10 /*See GlobeMetrics.OneIterationLength*/, 1)]
        [TestCase(20, 2)]
        [TestCase(100, 10)]
        [TestCase(9, 0)] // Globe updates are less that full itaration. So transition handler isn't update.
        [TestCase(11, 1)] // Only 1 full globe iteration completes.
        [TestCase(21, 2)]
        public async Task UpdateAsyncTest_UpdateTransitionOnceOfGlobeIteration_GlobeTransitionHandlersUpdateCalled(
            int globeUpdatesCount,
            int expectedHandlersUpdateCount)
        {
            // ARRANGE

            var transitionHandlerMock = new Mock<IGlobeTransitionHandler>();
            var transitionHandler = transitionHandlerMock.Object;

            var globe = new Globe(transitionHandler);

            // ACT

            for (var i = 0; i < globeUpdatesCount; i++)
            {
                await globe.UpdateAsync(CancellationToken.None).ConfigureAwait(false);
            }

            // ASSERT

            transitionHandlerMock.Verify(x => x.UpdateTransitions(), Times.Exactly(expectedHandlersUpdateCount));
        }
    }
}