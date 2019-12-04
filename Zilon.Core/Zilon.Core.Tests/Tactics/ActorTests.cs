using System;
using Moq;
using NUnit.Framework;
using Zilon.Core.Graphs;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture()]
    public class ActorTests
    {
        /// <summary>
        /// Этот юнит тест нуждается в переработке.
        /// Он нужен, чтобы отловить баг с поглощением злой тыквы, которая является шлемом.
        /// </summary>
        [Test()]
        [Category("NOT_unit")]
        public void UsePropTest()
        {
            var person = new Mock<IPerson>().Object;
            var player = new Mock<IPlayer>().Object;
            var node = new Mock<IGraphNode>().Object;

            var actor = new Actor(person, player, node);

            var schemeFactory = new SchemeServiceHandlerFactory(CreateSchemeLocator());
            var schemeService = new SchemeService(schemeFactory);
            var propfactory = new PropFactory(schemeService);

            var testProp = propfactory.CreateEquipment(schemeService.GetScheme<IPropScheme>("evil-pumpkin"));

            actor.UseProp(testProp);
        }

        /// <summary>
        /// Тест проверяет, что правило снижения токсикации снижает характеристику токсикации.
        /// </summary>
        [Test]
        public void UserProp_ComsumableWithDetoxication_ReduceIntoxication()
        {
            // ARRANGE
            var survivalMock = new Mock<ISurvivalData>();
            var survival = survivalMock.Object;

            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.Survival).Returns(survival);
            var person = personMock.Object;

            var player = new Mock<IPlayer>().Object;
            var node = new Mock<IGraphNode>().Object;

            var actor = new Actor(person, player, node);

            var testPropScheme = new TestPropScheme
            {
                Use = new TestPropUseSubScheme
                {
                    CommonRules = new[] {
                        new ConsumeCommonRule(
                            ConsumeCommonRuleType.Intoxication,
                            Components.PersonRuleLevel.Lesser,
                            Components.PersonRuleDirection.Negative)
                    }
                }
            };
            var testResource = new Resource(testPropScheme, 1);

            // ACT
            actor.UseProp(testResource);

            // ASSERT
            survivalMock.Verify(x => x.DecreaseStat(It.Is<SurvivalStatType>(v => v == SurvivalStatType.Intoxication),
                It.IsAny<int>()));
        }

        private FileSchemeLocator CreateSchemeLocator()
        {
            var schemeLocator = FileSchemeLocator.CreateFromEnvVariable();
            return schemeLocator;
        }
    }
}