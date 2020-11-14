using Zilon.Core.Components;
using Zilon.Core.Graphs;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture]
    public class ActorTests
    {
        /// <summary>
        /// Тест проверяет, что правило снижения токсикации снижает характеристику токсикации.
        /// </summary>
        [Test]
        public void UserProp_ComsumableWithDetoxication_ReduceIntoxication()
        {
            // ARRANGE
            var survivalModuleMock = new Mock<ISurvivalModule>();
            var survivalModule = survivalModuleMock.Object;

            var personMock = new Mock<IPerson>();
            personMock.Setup(x => x.GetModule<ISurvivalModule>(It.IsAny<string>())).Returns(survivalModule);
            personMock.Setup(x => x.HasModule(It.IsAny<string>())).Returns(true);
            var person = personMock.Object;

            var node = new Mock<IGraphNode>().Object;

            var taskSourceMock = new Mock<IActorTaskSource<ISectorTaskSourceContext>>();
            var taskSource = taskSourceMock.Object;

            var actor = new Actor(person, taskSource, node);

            var testPropScheme = new TestPropScheme
            {
                Use = new TestPropUseSubScheme
                {
                    CommonRules = new[]
                    {
                        new ConsumeCommonRule(
                            ConsumeCommonRuleType.Intoxication,
                            PersonRuleLevel.Lesser,
                            PersonRuleDirection.Negative)
                    }
                }
            };
            var testResource = new Resource(testPropScheme, 1);

            // ACT
            actor.UseProp(testResource);

            // ASSERT
            survivalModuleMock.Verify(x =>
                x.DecreaseStat(It.Is<SurvivalStatType>(v => v == SurvivalStatType.Intoxication),
                    It.IsAny<int>()));
        }
    }
}