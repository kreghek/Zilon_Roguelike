using System;
using System.Collections.Generic;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture][Parallelizable(ParallelScope.All)]
    public class GameLoopTests
    {
        /// <summary>
        /// Тест проверяет, то не выбрасывается исключений при обновлении игрового цикла.
        /// </summary>
        [Test]
        public void Update_PlayerAndMonster_NotThrowException()
        {
            // ASSERT
            var sectorMock = new Mock<ISector>();
            var sector = sectorMock.Object;

            var sectorManagerMock = new Mock<ISectorManager>();
            sectorManagerMock.SetupGet(x => x.CurrentSector).Returns(sector);
            var sectorManager = sectorManagerMock.Object;

            var actorManagerMock = new Mock<IActorManager>();
            var actorInnerList = new List<IActor>();
            actorManagerMock.SetupGet(x => x.Items).Returns(actorInnerList);
            var actorManager = actorManagerMock.Object;
            sectorMock.SetupGet(x => x.ActorManager).Returns(actorManager);

            var humanPlayer = new HumanPlayer();
            var humanActor = CreateActor(humanPlayer);
            actorInnerList.Add(humanActor);

            var botPlayer = new BotPlayer();
            var botActor = CreateActor(botPlayer);
            actorInnerList.Add(botActor);

            var gameLoop = new GameLoop(sectorManager)
            {
                ActorTaskSources = Array.Empty<IActorTaskSource>()
            };



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

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;
            actorMock.SetupGet(x => x.Person).Returns(person);

            var survivalDataMock = new Mock<ISurvivalModule>();
            var survivalData = survivalDataMock.Object;
            personMock.SetupGet(x => x.GetModule<ISurvivalModule>(It.IsAny<string>())).Returns(survivalData);

            return actor;
        }
    }
}