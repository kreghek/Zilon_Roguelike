using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tactics.Behaviour.Tests
{
    [TestFixture]
    public class UsePropHelperTests
    {
        [Test]
        [TestCaseSource(typeof(UsePropHelperTestCaseSource),
            nameof(UsePropHelperTestCaseSource.NoCriticalEffectTestCases))]
        public void CheckPropAllowedByRestrictions_HasMaxSurivalHazardEffect_UsageIsNoAllowed(
            SurvivalStatType effectStatType, UsageRestrictionRule usageRule)
        {
            // ARRANGE

            var personMock = new Mock<IPerson>();

            var сonditionsModuleMock = new Mock<IConditionModule>();
            var сonditions = new[]
            {
                new SurvivalStatHazardEffect(effectStatType, SurvivalStatHazardLevel.Max,
                    Mock.Of<ISurvivalRandomSource>())
            };
            сonditionsModuleMock.Setup(x => x.Items).Returns(сonditions);
            personMock.Setup(x => x.GetModule<IConditionModule>(nameof(IConditionModule)))
                .Returns(сonditionsModuleMock.Object);
            personMock.Setup(x => x.HasModule(nameof(IConditionModule))).Returns(true);

            var actor = Mock.Of<IActor>(x => x.Person == personMock.Object);

            var propScheme = new TestPropScheme
            {
                Use = new TestPropUseSubScheme
                {
                    Restrictions = Mock.Of<IUsageRestrictions>(x =>
                        x.Items == new[] { Mock.Of<IUsageRestrictionItem>(item => item.Type == usageRule) })
                }
            };
            var prop = new Resource(propScheme, 1);

            var context = Mock.Of<IActorTaskContext>();

            // ACT

            var fact = UsePropHelper.CheckPropAllowedByRestrictions(prop, actor, context);

            // ASSERT
            fact.Should().BeFalse();
        }
    }
}