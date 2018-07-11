using FluentAssertions;

using NUnit.Framework;

using System;

namespace Zilon.Core.Persons.Tests
{
    [TestFixture()]
    public class MonsterPersonTests
    {
        [Test()]
        public void Constructor_DefaultParams_NoException()
        {
            // ACT
            Action act = () => { var monster = new MonsterPerson(); };



            // ARRANGE
            act.Should().NotThrow();
        }
    }
}