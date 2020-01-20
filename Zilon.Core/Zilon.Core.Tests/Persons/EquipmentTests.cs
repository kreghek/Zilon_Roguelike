using System;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Persons
{

    [TestFixture][Parallelizable(ParallelScope.All)]
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
            var scheme = new TestPropScheme {
                Equip = null //Явно указываем, что предмет не является экипировкой.
            };

            var acts = new TacticalActScheme[0];


            // ACT
            Action act = () =>
            {
                // ReSharper disable once UnusedVariable
                var equipment = new Equipment(scheme, acts);
            };



            // ASSERT
            act.Should().Throw<ArgumentException>();
        }
    }
}