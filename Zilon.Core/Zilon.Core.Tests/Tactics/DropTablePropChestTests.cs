using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    public class DropTablePropChestTests
    {
        /// <summary>
        /// Тест проверяет генерацию предметов в контейнере,
        /// если по таблице дропа гарантированно должен выпасть указанный предмет.
        /// </summary>
        [Test]
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

            var testPropScheme = new PropScheme
            {
                Sid = "test-prop"
            };

            var dropResolverMock = new Mock<IDropResolver>();
            dropResolverMock.Setup(x => x.GetProps(It.IsAny<IEnumerable<DropTableScheme>>()))
                .Returns(new IProp[] { new Resource(testPropScheme, 1) });
            var dropResolver = dropResolverMock.Object;

            var container = new DropTablePropChest(node,
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