using FluentAssertions;

using Moq;

using NUnit.Framework;

using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture]
    public class DropResolverTests
    {
        [Test]
        public void GetPropsTest()
        {
            // ARRANGE

            const string testPropSchemeSid = "test-resource";

            var testResourceScheme = new PropScheme {
                Sid = testPropSchemeSid
            };

            var randomSourceMock = new Mock<IDropResolverRandomSource>();
            randomSourceMock.Setup(x => x.RollWeight(It.IsAny<int>()))
                .Returns(1);
            randomSourceMock.Setup(x => x.RollResourceCount(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(1);
            var randomSource = randomSourceMock.Object;

            var schemeServiceMock = new Mock<ISchemeService>();
            schemeServiceMock.Setup(x => x.GetScheme<PropScheme>(It.Is<string>(sid => sid == testPropSchemeSid)))
                .Returns(testResourceScheme);
            var schemeService = schemeServiceMock.Object;

            var propFactoryMock = new Mock<IPropFactory>();
            var propFactory = propFactoryMock.Object;

            var resolver = new DropResolver(randomSource, schemeService, propFactory);

            var testDropTableRecord = new DropTableRecordSubScheme(testPropSchemeSid, 1)
            {
                MinCount = 1,
                MaxCount = 1
            };

            var testDropTable = new DropTableScheme(1, testDropTableRecord);


            // ACT
            var factProps = resolver.GetProps(new[] { testDropTable });



            // ASSERT
            factProps.Count().Should().Be(1);
            factProps[0].Scheme.Should().BeSameAs(testResourceScheme);
            ((Resource)factProps[0]).Count.Should().Be(1);
        }
    }
}