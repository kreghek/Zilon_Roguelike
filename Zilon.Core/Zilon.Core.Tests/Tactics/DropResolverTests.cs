using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    public class DropResolverTests
    {
        [Test]
        public void GetPropsTest()
        {
            // ARRANGE

            const string testPropSchemeSid = "test-resource";

            var testResourceScheme = new PropScheme
            {
                Sid = testPropSchemeSid
            };

            var randomSourceMock = new Mock<IDropResolverRandomSource>();
            randomSourceMock.Setup(x => x.RollWeight(It.IsAny<int>()))
                .Returns(1);
            randomSourceMock.Setup(x => x.RollResourceCount(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(1);
            var randomSource = randomSourceMock.Object;

            var schemeServiceMock = new Mock<ISchemeService>();
            schemeServiceMock.Setup(x => x.GetScheme<IPropScheme>(It.Is<string>(sid => sid == testPropSchemeSid)))
                .Returns(testResourceScheme);
            var schemeService = schemeServiceMock.Object;

            var propFactoryMock = new Mock<IPropFactory>();
            var propFactory = propFactoryMock.Object;

            var resolver = new DropResolver(randomSource, schemeService, propFactory);

            var testDropTableRecord = new TestDropTableRecordSubScheme
            {
                SchemeSid = testPropSchemeSid,
                Weight = 1,
                MinCount = 1,
                MaxCount = 1
            };

            var testDropTable = new TestDropTableScheme(1, testDropTableRecord);


            // ACT
            var factProps = resolver.GetProps(new[] { testDropTable });



            // ASSERT
            factProps.Length.Should().Be(1);
            factProps[0].Scheme.Should().BeSameAs(testResourceScheme);
            ((Resource)factProps[0]).Count.Should().Be(1);
        }
    }
}