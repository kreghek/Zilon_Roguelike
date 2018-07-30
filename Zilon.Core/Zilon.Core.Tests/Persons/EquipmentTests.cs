using System;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Persons
{

    [TestFixture()]
    public class EquipmentTests
    {
        /// <summary>
        /// Тест проверяет, что при констрировании экипировки выбрасывается ошибка,
        /// если использована схема без параметров экипировки.
        /// </summary>
        [Test]
        public void Equipment_SchemeWithOutEquipSubScheme_NoExceptions()
        {
            // ARRANGE
            var scheme = new PropScheme();

            var acts = new TacticalActScheme[0];


            // ACT
            Action act = () => {
                var equipment = new Equipment(scheme, acts);
            };



            // ASSERT
            act.Should().Throw<ArgumentException>();
        }
    }
}