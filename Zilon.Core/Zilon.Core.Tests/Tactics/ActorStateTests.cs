using NUnit.Framework;
using Zilon.Core.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.Persons;
using Moq;
using FluentAssertions;

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