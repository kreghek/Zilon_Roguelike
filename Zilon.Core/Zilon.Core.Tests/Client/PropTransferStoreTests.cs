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
        ///     Тест проверяет, что при добавлении в пустой инвентарь 1 единицы ресурса
        ///     на выходе будет 1 единица этого ресурса.
        /// </summary>
        [Test]
        public void Add_AddResourceToEmptyInventory_PropContainsThisResource()
        {
            // ARRANGE
            const int expectedCount = 1;

            IProp[] props = new IProp[0];
            IPropStore realStore = CreateContainer(props);

            PropScheme testedScheme = new PropScheme();
            Resource testedResource = new Resource(testedScheme, expectedCount);

            PropTransferStore propTransferStore = new PropTransferStore(realStore);


            // ACT
            propTransferStore.Add(testedResource);


            // ASSERT
            IProp[] factProps = propTransferStore.CalcActualItems();
            factProps[0].Should().BeOfType<Resource>();
            factProps[0].Scheme.Should().Be(testedScheme);
            ((Resource)factProps[0]).Count.Should().Be(expectedCount);
        }

        /// <summary>
        ///     Тест проверяет, что при удалении 1 единицы ресурса из инвентаря с 1 единицей этого ресурса
        ///     на выходе будет пустой инвентярь.
        /// </summary>
        [Test]
        public void Remove_RemoveResourceFromInventoryWithThisResource_PropContains()
        {
            // ARRANGE
            const int inventoryCount = 1;
            const int expectedCount = 1;

            PropScheme testedScheme = new PropScheme();

            IProp[] props = {new Resource(testedScheme, inventoryCount)};

            IPropStore realStore = CreateContainer(props);


            Resource testedResource = new Resource(testedScheme, expectedCount);

            PropTransferStore propTransferStore = new PropTransferStore(realStore);


            // ACT
            propTransferStore.Remove(testedResource);


            // ASSERT
            IProp[] factProps = propTransferStore.CalcActualItems();
            factProps.Should().BeEmpty();
        }

        /// <summary>
        ///     Тест проверяет, что при добавлении 1 единицы ресурса в инвентарь с 1 единицей этого ресурса
        ///     на выходе будет 2 единицы этого ресурса.
        /// </summary>
        [Test]
        public void Add_AddResourceToInventoryWithThisResource_PropContains2UnitsOfResource()
        {
            // ARRANGE
            const int inventoryCount = 1;
            const int testedCount = 1;
            const int expectedCount = inventoryCount + testedCount;

            PropScheme testedScheme = new PropScheme();

            IProp[] props = {new Resource(testedScheme, inventoryCount)};

            IPropStore realStore = CreateContainer(props);


            Resource testedResource = new Resource(testedScheme, testedCount);

            PropTransferStore propTransferStore = new PropTransferStore(realStore);


            // ACT
            propTransferStore.Add(testedResource);


            // ASSERT
            IProp[] factProps = propTransferStore.CalcActualItems();
            factProps.Length.Should().Be(1); // В инвентаре только один стак ресурсов.
            factProps[0].Should().BeOfType<Resource>();
            factProps[0].Scheme.Should().Be(testedScheme);
            ((Resource)factProps[0]).Count.Should().Be(expectedCount);
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