using System;
using System.Collections.Generic;
using System.Text;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons.Auxiliary;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Scoring;

namespace Zilon.Core.Persons.Auxiliary.Tests
{
    [TestFixture]
    public class PersonEffectHelperTests
    {
        /// <summary>
        /// Test checks new effect was added if some of segments was crossed.
        /// </summary>
        [Test]
        public void UpdateSurvivalEffect_EmptyEffectsAndCrossKeyPoint_NewEffectWasAdded()
        {
            // ARRANGE

            var сonditionModuleMock = new Mock<IConditionsModule>();
            сonditionModuleMock.SetupGet(x => x.Items).Returns(Array.Empty<IPersonCondition>());
            var сonditionModule = сonditionModuleMock.Object;

            var stat = new SurvivalStat(0, 0, 1);
            var segments = new[] { new SurvivalStatKeySegment(0, 1, SurvivalStatHazardLevel.Lesser) };
            var randomSource = Mock.Of<ISurvivalRandomSource>();
            var eventLogService = Mock.Of<IPlayerEventLogService>();

            // ACT
            using var monitor = сonditionModule.Monitor();

            PersonConditionHelper.UpdateSurvivalСondition(сonditionModule, stat, segments, randomSource,
                eventLogService);

            // ARRANGE
            сonditionModuleMock.Verify(x => x.Add(It.IsAny<IPersonCondition>()));
        }
    }
}