﻿using System.Collections;

using NUnit.Framework;

using Zilon.Core.World;

namespace Zilon.Core.PersonModules.Tests
{
    public static class MonsterMovingModuleTestCaseSource
    {
        public static IEnumerable PositiveTestCases
        {
            get
            {
                // Factor
                // Returns move cost

                // The tests created for iteration length is even.

                yield return new TestCaseData(1f).Returns(GlobeMetrics.OneIterationLength);
                yield return new TestCaseData(2f).Returns(GlobeMetrics.OneIterationLength / 2);
                yield return new TestCaseData(0.5f).Returns(GlobeMetrics.OneIterationLength * 2);
                yield return new TestCaseData(0.1f).Returns(GlobeMetrics.OneIterationLength * 10);
                yield return new TestCaseData(10f).Returns(GlobeMetrics.OneIterationLength / 10);

                yield return new TestCaseData(float.MaxValue).Returns(GlobeMetrics.MinMonsterMoveCost);
                yield return new TestCaseData(0.00001f).Returns(GlobeMetrics.MaxMonsterMoveCost);
            }
        }
    }
}