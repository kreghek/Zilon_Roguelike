using System.Linq;
using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Client
{
    [TestFixture]
    public class PropTransferMachineTests
    {
        /// <summary>
        /// Тест проверяет корректность переноса ресурса из сундука в инвентарь.
        /// </summary>
        [Test]
        public void TransferProp_Resources()
        {
            // ARRANGE

            var resourceScheme = new TestPropScheme();

            // Инвентарь
            var inventory = new Inventory();

            // контейнер
            var containerProps = new IProp[] {
                new Resource(resourceScheme, 1)
            };
            var nodeMock = new Mock<IMapNode>();
            var node = nodeMock.Object;
            var container = new FixedPropChest(node, containerProps);

            // трансферная машина
            var transferMachine = new PropTransferMachine(inventory, container.Content);



            // ACT
            var transferResource = new Resource(resourceScheme, 1);
            transferMachine.TransferProp(transferResource,
                PropTransferMachineStores.Container,
                PropTransferMachineStores.Inventory);



            // ASSERT
            transferMachine.Inventory.PropAdded[0].Should().BeOfType<Resource>();
            var invResource = (Resource)transferMachine.Inventory.PropAdded[0];
            invResource.Count.Should().Be(1);
        }


        /// <summary>
        /// Тест проверяет корректность переноса ресурса из сундука в инвентарь.
        /// </summary>
        [Test]
        public void TransferProp_Equipment()
        {
            // ARRANGE

            var equipmentScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme()
            };

            // Инвентарь
            var inventory = new Inventory();

            // контейнер
            var containerProps = new IProp[] {
                new Equipment(equipmentScheme, new ITacticalActScheme[0])
            };
            var nodeMock = new Mock<IMapNode>();
            var node = nodeMock.Object;
            var container = new FixedPropChest(node, containerProps);

            // трансферная машина
            var transferMachine = new PropTransferMachine(inventory, container.Content);



            // ACT
            var transferResource = containerProps.First();
            transferMachine.TransferProp(transferResource,
                PropTransferMachineStores.Container,
                PropTransferMachineStores.Inventory);



            // ASSERT
            transferMachine.Inventory.PropAdded[0].Should().BeOfType<Equipment>();
            transferMachine.Container.PropRemoved[0].Should().BeOfType<Equipment>();
        }

        /// <summary>
        /// Тест проверяет корректность отработчков событий при пененосе ресурса из сундука в инвентарь.
        /// </summary>
        [Test]
        public void TransferProp_Resources_StoreEventsRaised()
        {
            // ARRANGE

            var resourceScheme = new TestPropScheme();

            // Инвентарь
            var inventory = new Inventory();

            // контейнер
            var containerProps = new IProp[] {
                new Resource(resourceScheme, 1)
            };
            var nodeMock = new Mock<IMapNode>();
            var node = nodeMock.Object;
            var container = new FixedPropChest(node, containerProps);

            // трансферная машина
            var transferMachine = new PropTransferMachine(inventory, container.Content);



            // ACT
            using (var monitorInventory = transferMachine.Inventory.Monitor())
            using (var monitorContainer = transferMachine.Container.Monitor())
            {
                var transferResource = new Resource(resourceScheme, 1);
                transferMachine.TransferProp(transferResource,
                    PropTransferMachineStores.Container,
                    PropTransferMachineStores.Inventory);



                // ASSERT
                monitorInventory.Should().Raise(nameof(PropTransferStore.Added));
                monitorContainer.Should().Raise(nameof(PropTransferStore.Removed));
            }
        }

        /// <summary>
        /// Тест проверяет корректность отработчков событий при пененосе ресурса из сундука в инвентарь.
        /// </summary>
        [Test]
        public void TransferProp_InventoryHasResources_StoreEventsRaised()
        {
            // ARRANGE

            var resourceScheme = new TestPropScheme();

            // Инвентарь
            var inventory = new Inventory();
            inventory.Add(new Resource(resourceScheme, 1));

            // контейнер
            var containerProps = new IProp[] {
                new Resource(resourceScheme, 1)
            };
            var nodeMock = new Mock<IMapNode>();
            var node = nodeMock.Object;
            var container = new FixedPropChest(node, containerProps);

            // трансферная машина
            var transferMachine = new PropTransferMachine(inventory, container.Content);



            // ACT
            using (var monitorInventory = transferMachine.Inventory.Monitor())
            using (var monitorContainer = transferMachine.Container.Monitor())
            {
                var transferResource = new Resource(resourceScheme, 1);
                transferMachine.TransferProp(transferResource,
                    PropTransferMachineStores.Container,
                    PropTransferMachineStores.Inventory);



                // ASSERT
                monitorInventory.Should().Raise(nameof(PropTransferStore.Changed));
                monitorContainer.Should().Raise(nameof(PropTransferStore.Removed));
            }
        }

        /// <summary>
        /// Тест проверяет корректность отработчков событий при пененосе ресурса из сундука в инвентарь.
        /// </summary>
        [Test]
        public void TransferProp_ChangesResources_StoreEventsRaised()
        {
            // ARRANGE

            var resourceScheme = new TestPropScheme();

            // Инвентарь
            var inventory = new Inventory();
            inventory.Add(new Resource(resourceScheme, 1));

            // контейнер
            var containerProps = new IProp[] {
                new Resource(resourceScheme, 2)
            };
            var nodeMock = new Mock<IMapNode>();
            var node = nodeMock.Object;
            var container = new FixedPropChest(node, containerProps);

            // трансферная машина
            var transferMachine = new PropTransferMachine(inventory, container.Content);



            // ACT
            using (var monitorInventory = transferMachine.Inventory.Monitor())
            using (var monitorContainer = transferMachine.Container.Monitor())
            {
                var transferResource = new Resource(resourceScheme, 1);
                transferMachine.TransferProp(transferResource,
                    PropTransferMachineStores.Container,
                    PropTransferMachineStores.Inventory);



                // ASSERT
                monitorInventory.Should().Raise(nameof(PropTransferStore.Changed));
                monitorContainer.Should().Raise(nameof(PropTransferStore.Changed));
            }
        }

        /// <summary>
        /// Тест проверяет корректность отработчков событий при пененосе предмета экипировки из сундука в инвентарь.
        /// </summary>
        [Test]
        public void TransferProp_Equipment_StoreEventsRaised()
        {
            // ARRANGE

            var equipmentScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme()
            };

            // Инвентарь
            var inventory = new Inventory();

            // контейнер
            var containerProps = new IProp[] {
                new Equipment(equipmentScheme, new ITacticalActScheme[0])
            };
            var nodeMock = new Mock<IMapNode>();
            var node = nodeMock.Object;
            var container = new FixedPropChest(node, containerProps);

            // трансферная машина
            var transferMachine = new PropTransferMachine(inventory, container.Content);



            // ACT
            using (var monitorInventory = transferMachine.Inventory.Monitor())
            using (var monitorContainer = transferMachine.Container.Monitor())
            {
                var transferResource = containerProps.First();
                transferMachine.TransferProp(transferResource,
                    PropTransferMachineStores.Container,
                    PropTransferMachineStores.Inventory);



                // ASSERT
                monitorInventory.Should().Raise(nameof(PropTransferStore.Added));
                monitorContainer.Should().Raise(nameof(PropTransferStore.Removed));
            }
        }

        /// <summary>
        /// Тест проверяет корректность отработчков событий при пененосе предмета экипировки из сундука в инвентарь.
        /// В инвентаре уже есть предмет с такой схемой.
        /// </summary>
        [Test]
        public void TransferProp_InventoryHasEquipment_StoreEventsRaised()
        {
            // ARRANGE

            var equipmentScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme()
            };

            // Инвентарь
            var inventory = new Inventory();
            inventory.Add(new Equipment(equipmentScheme, new ITacticalActScheme[0]));

            // контейнер
            var containerProps = new IProp[] {
                new Equipment(equipmentScheme, new ITacticalActScheme[0])
            };
            var nodeMock = new Mock<IMapNode>();
            var node = nodeMock.Object;
            var container = new FixedPropChest(node, containerProps);

            // трансферная машина
            var transferMachine = new PropTransferMachine(inventory, container.Content);



            // ACT
            using (var monitorInventory = transferMachine.Inventory.Monitor())
            using (var monitorContainer = transferMachine.Container.Monitor())
            {
                var transferResource = containerProps.First();
                transferMachine.TransferProp(transferResource,
                    PropTransferMachineStores.Container,
                    PropTransferMachineStores.Inventory);



                // ASSERT
                monitorInventory.Should().Raise(nameof(PropTransferStore.Added));
                monitorContainer.Should().Raise(nameof(PropTransferStore.Removed));
            }
        }
    }
}