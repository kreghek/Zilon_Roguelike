using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    public class ActorStateTests
    {
        /// <summary>
        /// Тест проверяет, что при восстановлении Хп текущее значение не выходит за рамки максимального.
        /// </summary>
        [Test]
        public void RestoreHp_RestoreHp_HpNotGreaterThatMaxPersonHp()
        {
            // ARRANGE
            const int initialHp = 2;
            const int personHp = 3;
            const int restoreHpValue = 2;
            const int expectedHp = personHp;

            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.Hp).Returns(personHp);
            var person = personMock.Object;

            var actorState = new ActorState(person, initialHp);



            // ACT
            actorState.RestoreHp(restoreHpValue);



            // ASSERT
            actorState.Hp.Should().Be(expectedHp);
        }
    }
}