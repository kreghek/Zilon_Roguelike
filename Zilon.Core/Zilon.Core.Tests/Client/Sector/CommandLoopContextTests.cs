using System;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Client.Sector.Tests
{
    [TestFixture]
    public class CommandLoopContextTests
    {
        /// <summary>
        /// The test checks WaitForUpdate releses the awaiting then CanIntent() returns true.
        /// </summary>
        [Test]
        [Timeout(5000)]
        public async Task WaitForUpdateTest_WaitUntilCanIndentGetsTrueAfter200ms_WaitForUpdateReleased()
        {
            // ARRANGE

            var player = Mock.Of<IPlayer>(player => player.MainPerson == Mock.Of<IPerson>());

            var canIndent = false;
            var taskSourceMock = new Mock<IHumanActorTaskSource<ISectorTaskSourceContext>>();
            taskSourceMock.Setup(x => x.CanIntent()).Returns(() => { return canIndent; });
            var taskSource = taskSourceMock.Object;

            var commandLoopContext = new CommandLoopContext(player, taskSource);

            // ACT

            var updateTask = Task.Run(async () =>
            {
                await commandLoopContext.WaitForUpdate(CancellationToken.None);

                return true;
            });

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                await Task.Delay(200);

                canIndent = true;
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            var factUpdateResult = await updateTask;

            // ASSERT
            factUpdateResult.Should().BeTrue();
        }

        /// <summary>
        /// The test checks WaitForUpdate don't relese the awaiting then CanIntent() dont'n return true.
        /// </summary>
        [Test]
        [Timeout(5000)]
        public void WaitForUpdateTest_WaitUntilCanIndentWhichIsNotGetsTrue_WaitForUpdateTimeoutStopped()
        {
            // ARRANGE

            var player = Mock.Of<IPlayer>(player => player.MainPerson == Mock.Of<IPerson>());

            var taskSourceMock = new Mock<IHumanActorTaskSource<ISectorTaskSourceContext>>();
            var taskSource = taskSourceMock.Object;

            var commandLoopContext = new CommandLoopContext(player, taskSource);

            // ACT

            var updateTask = Task.Run(async () =>
            {
                await commandLoopContext.WaitForUpdate(CancellationToken.None);
            });

            Func<Task> act = async () =>
            {
                await updateTask.TimeoutAfter(1000);
            };

            // ASSERT
            act.Should().Throw<TimeoutException>();
        }
    }
}