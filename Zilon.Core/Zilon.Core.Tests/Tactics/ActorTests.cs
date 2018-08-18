using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture()]
    public class ActorTests
    {
        /// <summary>
        /// Проверяет, что актёр при потере всего здоровья выстреливает событие смерти.
        /// </summary>
        [Test()]
        public void TakeDamage_FatalDamage_FiresEvent()
        {
            // ARRANGE

            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.Hp).Returns(1);
            var person = personMock.Object;

            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var nodeMock = new Mock<IMapNode>();
            var node = nodeMock.Object;

            var testActor = new Actor(person, player, node);


            // ACT
            using (var monitor = testActor.Monitor())
            {
                testActor.TakeDamage(1);

                monitor.Should().Raise(nameof(Actor.Dead));
            }
        }
    }
}