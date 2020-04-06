using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Persons.Tests
{
    [TestFixture]
    public class PersonPerkInitializatorTests
    {
        /// <summary>
        /// Проверяет, что при наличии одной схемы начального перка создаётся объект перка с этой схемой.
        /// </summary>
        [Test]
        public void GetTest_OneBuildInPerk_ReturnsThisBuildInPerk()
        {
            var diceMock = new Mock<IDice>();
            var dice = diceMock.Object;

            var schemeServiceMock = new Mock<ISchemeService>();
            var perkSchemeList = new IPerkScheme[] {
                    new TestPerkScheme{
                        Sid = "test",
                        IsBuildIn = true
                    }
                };
            schemeServiceMock.Setup(x => x.GetSchemes<IPerkScheme>()).Returns(perkSchemeList);
            var schemeService = schemeServiceMock.Object;

            var perkInitializator = new PersonPerkInitializator(dice, schemeService);

            // ACT

            var factPerks = perkInitializator.Get();

            // ASSERT

            factPerks.Should().HaveCount(1);
            factPerks[0].Scheme.Should().Be(perkSchemeList[0]);
        }
    }
}