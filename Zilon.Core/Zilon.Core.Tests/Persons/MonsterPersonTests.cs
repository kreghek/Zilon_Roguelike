using System;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons.Tests
{
    [TestFixture()]
    public class MonsterPersonTests
    {
        [Test()]
        public void Constructor_DefaultParams_NoException()
        {
            // ARRANGE
            var monsterScheme = new MonsterScheme
            {
                PrimaryAct = new TacticalActStatsSubScheme
                {
                    Efficient = new Range<float>(1, 2)
                }
            };

            // ACT
            Action act = () => { var monster = new MonsterPerson(monsterScheme); };



            // ARRANGE
            act.Should().NotThrow();
        }
    }
}