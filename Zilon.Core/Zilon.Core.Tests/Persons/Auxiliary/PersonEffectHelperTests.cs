using NUnit.Framework;
using Zilon.Core.Persons.Auxiliary;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Scoring;
using FluentAssertions;

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

            var effectModuleMock = new Mock<IEffectsModule>();
            effectModuleMock.SetupGet(x=>x.Items).Returns(Array.Empty<IPersonEffect>());
            var effectModule = effectModuleMock.Object;

            var stat = new SurvivalStat(0, 0, 1);
            var segments = new[] { new SurvivalStatKeySegment(0, 1, SurvivalStatHazardLevel.Lesser) };
            var randomSource = Mock.Of<ISurvivalRandomSource>();
            var eventLogService = Mock.Of<IPlayerEventLogService>();

            // ACT
            using var monitor = effectModule.Monitor();

            PersonEffectHelper.UpdateSurvivalEffect(effectModule, stat, segments, randomSource, eventLogService);

            // ARRANGE
            effectModuleMock.Verify(x => x.Add(It.IsAny<IPersonEffect>()));
        }
    }
}