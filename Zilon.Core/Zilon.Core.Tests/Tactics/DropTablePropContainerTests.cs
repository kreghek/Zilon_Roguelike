using FluentAssertions;

using Moq;

using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture()]
    public class DropTablePropContainerTests
    {
        [Test()]
        public void DropTablePropContainerTest()
        {
            // ARRANGE
            var nodeMock = new Mock<IMapNode>();
            var node = nodeMock.Object;

            var dropTableRecord = new DropTableRecordSubScheme("test-prop", 1)
            {
                MinCount = 1,
                MaxCount = 1
            };

            var dropTable = new DropTableScheme(1, dropTableRecord);

            var diceMock = new Mock<IDice>();
            diceMock.Setup(x => x.Roll(It.IsAny<int>())).Returns<int>(n => n);
            var dice = diceMock.Object;

            var schemeServiceMock = new Mock<ISchemeService>();

            var testPropScheme = new PropScheme {
                Sid = "test-prop"
            };

            schemeServiceMock.Setup(x => x.GetScheme<PropScheme>(It.IsAny<string>()))
                .Returns(testPropScheme);
            var schemeService = schemeServiceMock.Object;

            var dropResolverMock = new Mock<IDropResolver>();
            dropResolverMock.Setup(x => x.GetProps(It.IsAny<IEnumerable<DropTableScheme>>()))
                .Returns(new IProp[] { new Resource(testPropScheme, 1) });
            var dropResolver = dropResolverMock.Object;

            var container = new DropTablePropContainer(node,
                new[] { dropTable },
                dropResolver);



            // ACT
            var factProps = container.Content.CalcActualItems();



            // ASSERT
            factProps.Count().Should().Be(1);
            factProps[0].Scheme.Should().BeSameAs(testPropScheme);
            ((Resource)factProps[0]).Count.Should().Be(1);
        }
    }
}