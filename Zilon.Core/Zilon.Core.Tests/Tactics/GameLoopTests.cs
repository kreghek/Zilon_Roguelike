using System;
using System.Collections.Generic;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture()]
    public class GameLoopTests
    {
        [Test()]
        public void UpdateTest()
        {
            // ASSERT
            var sectorMock = new Mock<ISector>();
            var sector = sectorMock.Object;

            var actorManagerMock = new Mock<IActorManager>();
            var actorInnerList = new List<IActor>();
            actorManagerMock.SetupGet(x => x.Actors).Returns(actorInnerList);
            var actorManager = actorManagerMock.Object;

            var humanPlayer = new HumanPlayer();
            var humanActor = CreateActor(humanPlayer);
            actorInnerList.Add(humanActor);

            var botPlayer = new BotPlayer();
            var botActor = CreateActor(botPlayer);
            actorInnerList.Add(botActor);

            var gameLoop = new GameLoop(sector, actorManager);
            gameLoop.ActorTaskSources = new IActorTaskSource[0];



            // ACT
            Action act = () => { gameLoop.Update(); };



            // ARRANGE
            act.Should().NotThrow();
        }

        private IActor CreateActor(IPlayer player)
        {
            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Owner).Returns(player);
            var actor = actorMock.Object;

            var stateMock = new Mock<IActorState>();
            var state = stateMock.Object;
            actorMock.SetupGet(x => x.State).Returns(state);

            return actor;
        }
    }
}