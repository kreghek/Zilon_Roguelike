using System;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons.Tests
{
    [TestFixture()]
    public class EquipmentTests
    {
        [Test()]
        public void EquipmentTest()
        {
            // ARRANGE
            var scheme = new PropScheme();


            // ACT
            Action act = () => {
                var equipment = new Equipment(scheme);
            };



            // ASSERT
            act.Should().Throw<ArgumentException>();
        }
    }
}