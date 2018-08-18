using System;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Persons
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
            Action act = () => {
                var monster = new MonsterPerson(monsterScheme);
            };



            // ARRANGE
            act.Should().NotThrow();
        }
    }
}