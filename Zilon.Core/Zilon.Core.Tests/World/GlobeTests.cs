using System.Threading;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

namespace Zilon.Core.World.Tests
{
    [TestFixture]
    public class GlobeTests
    {
        /// <summary>
        /// The test checks invocation of globe transition handler. Every completed iteration the globe must call UpdateAsync of the globe transition handler.
        /// </summary>
        /// <param name="globeUpdatesCount"> Count of the globe's update calls. </param>
        /// <param name="expectedHandlersUpdateCount"> Expected count of update invocation of the globe transition handler's update. </param>
        /// <returns></returns>
        [Test]
        [Timeout(5000)]
        [TestCase(10 /*See GlobeMetrics.OneIterationLength*/, 1)]
        [TestCase(20, 2)]
        [TestCase(100, 10)]
        [TestCase(9, 0)]
        [TestCase(11, 1)]
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