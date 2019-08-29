using System.Collections;

using NUnit.Framework;

using Zilon.Core.Persons;

namespace Zilon.Core.Tests.Persons.TestCases
{
    public static class ISurvivalDataExtensionsTestCaseSource
    {
        public static IEnumerable CalcKeyPointTestCases {
            get {
                // Формат тестовых данных.
                // Значения 3 ключевых точек в порядке увеличения уровня Lesser, Strong, Max.
                // Проверяемый диапазон min, max.
                // Nullable для ключевых точек, которые должны быть пройдены.

                // Нужно помнить, что ключевые точки сейчас могут быть расположены только либо с лева от 0.
                // Либо справа от нуля. См. summary для ключевых точек.

                yield return new TestCaseData(25, 50, 100,
                    0, 51,
                    SurvivalStatHazardLevel.Lesser,
                    SurvivalStatHazardLevel.Strong,
                    null);

                yield return new TestCaseData(25, 50, 100,
                    99, 100,
                    SurvivalStatHazardLevel.Max,
                    null,
                    null);

                yield return new TestCaseData(1, 2, 3,
                    0, 3,
                    SurvivalStatHazardLevel.Lesser,
                    SurvivalStatHazardLevel.Strong,
                    SurvivalStatHazardLevel.Max);

                yield return new TestCaseData(-25, -50, -100,
                    0, -51,
                    SurvivalStatHazardLevel.Lesser,
                    SurvivalStatHazardLevel.Strong,
                    null);
            }
        }
    }
}
