using System.Collections;

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
        [TestCaseSource(typeof(UsePropHelperTestCaseSource), nameof(UsePropHelperTestCaseSource.NoCriticalEffectTestCases))]
        public void CheckPropAllowedByRestrictions_HasMaxSurivalHazardEffect_UsageIsNoAllowed(SurvivalStatType effectStatType, UsageRestrictionRule usageRule)
        {
            // ARRANGE

            var personMock = new Mock<IPerson>();

            var effectsModuleMock = new Mock<IEffectsModule>();
            var effects = new[] { new SurvivalStatHazardEffect(effectStatType, SurvivalStatHazardLevel.Max, Mock.Of<ISurvivalRandomSource>())};
            effectsModuleMock.Setup(x => x.Items).Returns(effects);
            personMock.Setup(x => x.GetModule<IEffectsModule>(nameof(IEffectsModule))).Returns(effectsModuleMock.Object);
            personMock.Setup(x => x.HasModule(nameof(IEffectsModule))).Returns(true);

            var actor = Mock.Of<IActor>(x => x.Person == personMock.Object);

            var propScheme = new TestPropScheme {
                Use = new TestPropUseSubScheme {
                    Restrictions = Mock.Of<IUsageRestrictions>(x => x.Items == new[] { Mock.Of<IUsageRestrictionItem>(item => item.Type == usageRule) })
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

    public static class UsePropHelperTestCaseSource
    { 
        public static IEnumerable NoCriticalEffectTestCases {
            get {
                yield return new TestCaseData(SurvivalStatType.Satiety, UsageRestrictionRule.NoStarvation);
                yield return new TestCaseData(SurvivalStatType.Hydration, UsageRestrictionRule.NoDehydration);
                yield return new TestCaseData(SurvivalStatType.Intoxication, UsageRestrictionRule.NoOverdose);
            }
        }
    }
}