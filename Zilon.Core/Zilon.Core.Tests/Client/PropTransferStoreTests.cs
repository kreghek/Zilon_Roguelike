using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Client
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class PropTransferStoreTests
    {
        /// <summary>
        /// Тест проверяет, что при добавлении в пустой инвентарь 1 единицы ресурса
        /// на выходе будет 1 единица этого ресурса.
        /// </summary>
        [Test]
        public void Add_AddResourceToEmptyInventory_PropContainsThisResource()
        {
            // ARRANGE
            const int expectedCount = 1;

            var props = new IProp[0];
            var realStore = CreateContainer(props);

            var testedScheme = new PropScheme();
            var testedResource = new Resource(testedScheme, expectedCount);

            var propTransferStore = new PropTransferStore(realStore);

            // ACT
            propTransferStore.Add(testedResource);

            // ASSERT
            var factProps = propTransferStore.CalcActualItems();
            factProps[0].Should().BeOfType<Resource>();
            factProps[0].Scheme.Should().Be(testedScheme);
            ((Resource)factProps[0]).Count.Should().Be(expectedCount);
        }

        /// <summary>
        /// Тест проверяет, что при добавлении 1 единицы ресурса в инвентарь с 1 единицей этого ресурса
        /// на выходе будет 2 единицы этого ресурса.
        /// </summary>
        [Test]
        public void Add_AddResourceToInventoryWithThisResource_PropContains2UnitsOfResource()
        {
            // ARRANGE
            const int inventoryCount = 1;
            const int testedCount = 1;
            const int expectedCount = inventoryCount + testedCount;

            var testedScheme = new PropScheme();

            var props = new IProp[]
            {
                new Resource(testedScheme, inventoryCount)
            };

            var realStore = CreateContainer(props);

            var testedResource = new Resource(testedScheme, testedCount);

            var propTransferStore = new PropTransferStore(realStore);

            // ACT
            propTransferStore.Add(testedResource);

            // ASSERT
            var factProps = propTransferStore.CalcActualItems();
            factProps.Length.Should().Be(1); // В инвентаре только один стак ресурсов.
            factProps[0].Should().BeOfType<Resource>();
            factProps[0].Scheme.Should().Be(testedScheme);
            ((Resource)factProps[0]).Count.Should().Be(expectedCount);
        }

        /// <summary>
        /// Тест проверяет, что при удалении 1 единицы ресурса из инвентаря с 1 единицей этого ресурса
        /// на выходе будет пустой инвентярь.
        /// </summary>
        [Test]
        public void Remove_RemoveResourceFromInventoryWithThisResource_PropContains()
        {
            // ARRANGE
            const int inventoryCount = 1;
            const int expectedCount = 1;

            var testedScheme = new PropScheme();

            var props = new IProp[]
            {
                new Resource(testedScheme, inventoryCount)
            };

            var realStore = CreateContainer(props);

            var testedResource = new Resource(testedScheme, expectedCount);

            var propTransferStore = new PropTransferStore(realStore);

            // ACT
            propTransferStore.Remove(testedResource);

            // ASSERT
            var factProps = propTransferStore.CalcActualItems();
            factProps.Should().BeEmpty();
        }

        private static IPropStore CreateContainer(IProp[] props)
        {
            var realStoreMock = new Mock<IPropStore>();
            realStoreMock.Setup(x => x.CalcActualItems()).Returns(props);
            var realStore = realStoreMock.Object;
            return realStore;
        }
    }
}