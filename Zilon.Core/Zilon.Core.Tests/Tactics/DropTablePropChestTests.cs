using System.Collections.Generic;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture][Parallelizable(ParallelScope.All)]
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

            var dropTableRecord = new TestDropTableRecordSubScheme
            {
                SchemeSid = "test-prop",
                Weight = 1,
                MinCount = 1,
                MaxCount = 1
            };

            var dropTable = new TestDropTableScheme(1, dropTableRecord);

            var testPropScheme = new PropScheme
            {
                Sid = "test-prop"
            };

            var dropResolverMock = new Mock<IDropResolver>();
            dropResolverMock.Setup(x => x.Resolve(It.IsAny<IEnumerable<IDropTableScheme>>()))
                .Returns(new IProp[] { new Resource(testPropScheme, 1) });
            var dropResolver = dropResolverMock.Object;

            var container = new DropTablePropChest(node,
                new IDropTableScheme[] { dropTable },
                dropResolver);



            // ACT
            var factProps = container.Content.CalcActualItems();



            // ASSERT
            factProps.Length.Should().Be(1);
            factProps[0].Scheme.Should().BeSameAs(testPropScheme);
            ((Resource)factProps[0]).Count.Should().Be(1);
        }
    }
}