using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture()]
    public class ActorTests
    {
        /// <summary>
        /// Проверяет, что актёр при потере всего здоровья выстреливает событие смерти.
        /// </summary>
        [Test()]
        public void TakeDamageTest()
        {
            // ARRANGE

            var person = new Person {
                Damage = 1,
                Hp = 1
            };

            var node = new HexNode(0, 0);

            var testActor = new Actor(person, node);


            // ACT
            using (var monitor = testActor.Monitor())
            {
                testActor.TakeDamage(1);

                monitor.Should().Raise(nameof(Actor.OnDead));
            }
        }
    }
}