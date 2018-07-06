using System;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons.Tests
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

            var acts = new ITacticalAct[0];


            // ACT
            Action act = () => {
                var equipment = new Equipment(scheme, acts);
            };



            // ASSERT
            act.Should().Throw<ArgumentException>();
        }
    }
}