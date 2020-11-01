using System;
using System.Linq;
using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Client
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class PropTransferMachineTests
    {
        /// <summary>
        ///     Тест проверяет корректность переноса ресурса из сундука в инвентарь.
        /// </summary>
        [Test]
        public void TransferProp_Resources()
        {
            // ARRANGE

            TestPropScheme resourceScheme = new TestPropScheme();

            // Инвентарь
            InventoryModule inventory = new InventoryModule();

            // контейнер
            IProp[] containerProps = {new Resource(resourceScheme, 1)};

            FixedPropChest container = new FixedPropChest(containerProps);

            // трансферная машина
            PropTransferMachine transferMachine = new PropTransferMachine(inventory, container.Content);

            // ACT
            Resource transferResource = new Resource(resourceScheme, 1);
            transferMachine.TransferProp(transferResource,
                PropTransferMachineStore.Container,
                PropTransferMachineStore.Inventory);

            // ASSERT
            transferMachine.Inventory.PropAdded[0].Should().BeOfType<Resource>();
            Resource invResource = (Resource)transferMachine.Inventory.PropAdded[0];
            invResource.Count.Should().Be(1);
        }


        /// <summary>
        ///     Тест проверяет корректность переноса ресурса из сундука в инвентарь.
        /// </summary>
        [Test]
        public void TransferProp_Equipment()
        {
            // ARRANGE

            TestPropScheme equipmentScheme = new TestPropScheme {Equip = new TestPropEquipSubScheme()};

            // Инвентарь
            InventoryModule inventory = new InventoryModule();

            // контейнер
            IProp[] containerProps = {new Equipment(equipmentScheme, Array.Empty<ITacticalActScheme>())};

            FixedPropChest container = new FixedPropChest(containerProps);

            // трансферная машина
            PropTransferMachine transferMachine = new PropTransferMachine(inventory, container.Content);

            // ACT
            IProp transferResource = containerProps.First();
            transferMachine.TransferProp(transferResource,
                PropTransferMachineStore.Container,
                PropTransferMachineStore.Inventory);

            // ASSERT
            transferMachine.Inventory.PropAdded[0].Should().BeOfType<Equipment>();
            transferMachine.Container.PropRemoved[0].Should().BeOfType<Equipment>();
        }

        /// <summary>
        ///     Тест проверяет корректность отработчков событий при пененосе ресурса из сундука в инвентарь.
        /// </summary>
        [Test]
        public void TransferProp_Resources_StoreEventsRaised()
        {
            // ARRANGE

            TestPropScheme resourceScheme = new TestPropScheme();
            Resource resource = new Resource(resourceScheme, 1);

            // Инвентарь
            InventoryModule inventory = new InventoryModule();

            // контейнер
            IProp[] containerProps = {resource};

            FixedPropChest container = new FixedPropChest(containerProps);

            // трансферная машина
            PropTransferMachine transferMachine = new PropTransferMachine(inventory, container.Content);

            // ACT
            using var monitorInventory = transferMachine.Inventory.Monitor();
            using var monitorContainer = transferMachine.Container.Monitor();
            Resource transferResource = new Resource(resourceScheme, 1);
            transferMachine.TransferProp(transferResource,
                PropTransferMachineStore.Container,
                PropTransferMachineStore.Inventory);


            // ASSERT
            monitorInventory.Should().Raise(nameof(PropTransferStore.Added))
                .WithArgs<PropStoreEventArgs>(args => args.Props[0].Scheme == resource.Scheme);
            monitorContainer.Should().Raise(nameof(PropTransferStore.Removed))
                .WithArgs<PropStoreEventArgs>(args => args.Props[0].Scheme == resource.Scheme);
        }

        /// <summary>
        ///     Тест проверяет корректность отработчков событий при пененосе ресурса из сундука в инвентарь.
        /// </summary>
        [Test]
        public void TransferProp_InventoryHasResources_StoreEventsRaised()
        {
            // ARRANGE

            TestPropScheme resourceScheme = new TestPropScheme();

            // Инвентарь
            InventoryModule inventory = new InventoryModule();
            inventory.Add(new Resource(resourceScheme, 1));

            // контейнер
            IProp[] containerProps = {new Resource(resourceScheme, 1)};

            FixedPropChest container = new FixedPropChest(containerProps);

            // трансферная машина
            PropTransferMachine transferMachine = new PropTransferMachine(inventory, container.Content);

            // ACT
            using var monitorInventory = transferMachine.Inventory.Monitor();
            using var monitorContainer = transferMachine.Container.Monitor();
            Resource transferResource = new Resource(resourceScheme, 1);
            transferMachine.TransferProp(transferResource,
                PropTransferMachineStore.Container,
                PropTransferMachineStore.Inventory);

            // ASSERT
            monitorInventory.Should().Raise(nameof(PropTransferStore.Changed));
            monitorContainer.Should().Raise(nameof(PropTransferStore.Removed));
        }

        /// <summary>
        ///     Тест проверяет корректность отработчков событий при пененосе ресурса из сундука в инвентарь.
        /// </summary>
        [Test]
        public void TransferProp_ChangesResources_StoreEventsRaised()
        {
            // ARRANGE

            TestPropScheme resourceScheme = new TestPropScheme();

            // Инвентарь
            InventoryModule inventory = new InventoryModule();
            inventory.Add(new Resource(resourceScheme, 1));

            // контейнер
            IProp[] containerProps = {new Resource(resourceScheme, 2)};

            FixedPropChest container = new FixedPropChest(containerProps);

            // трансферная машина
            PropTransferMachine transferMachine = new PropTransferMachine(inventory, container.Content);

            // ACT
            using var monitorInventory = transferMachine.Inventory.Monitor();
            using var monitorContainer = transferMachine.Container.Monitor();
            Resource transferResource = new Resource(resourceScheme, 1);
            transferMachine.TransferProp(transferResource,
                PropTransferMachineStore.Container,
                PropTransferMachineStore.Inventory);

            // ASSERT
            monitorInventory.Should().Raise(nameof(PropTransferStore.Changed));
            monitorContainer.Should().Raise(nameof(PropTransferStore.Changed));
        }

        /// <summary>
        ///     Тест проверяет корректность отработчков событий при пененосе предмета экипировки из сундука в инвентарь.
        /// </summary>
        [Test]
        public void TransferProp_Equipment_StoreEventsRaised()
        {
            // ARRANGE

            TestPropScheme equipmentScheme = new TestPropScheme {Equip = new TestPropEquipSubScheme()};

            // Инвентарь
            InventoryModule inventory = new InventoryModule();

            // контейнер
            IProp[] containerProps = {new Equipment(equipmentScheme, Array.Empty<ITacticalActScheme>())};

            FixedPropChest container = new FixedPropChest(containerProps);

            // трансферная машина
            PropTransferMachine transferMachine = new PropTransferMachine(inventory, container.Content);

            // ACT
            using var monitorInventory = transferMachine.Inventory.Monitor();
            using var monitorContainer = transferMachine.Container.Monitor();
            IProp transferResource = containerProps.First();
            transferMachine.TransferProp(transferResource,
                PropTransferMachineStore.Container,
                PropTransferMachineStore.Inventory);

            // ASSERT
            monitorInventory.Should().Raise(nameof(PropTransferStore.Added));
            monitorContainer.Should().Raise(nameof(PropTransferStore.Removed));
        }

        /// <summary>
        ///     Тест проверяет корректность отработчков событий при пененосе предмета экипировки из сундука в инвентарь.
        ///     В инвентаре уже есть предмет с такой схемой.
        /// </summary>
        [Test]
        public void TransferProp_InventoryHasEquipment_StoreEventsRaised()
        {
            // ARRANGE

            TestPropScheme equipmentScheme = new TestPropScheme {Equip = new TestPropEquipSubScheme()};

            // Инвентарь
            InventoryModule inventory = new InventoryModule();
            inventory.Add(new Equipment(equipmentScheme, Array.Empty<ITacticalActScheme>()));

            // контейнер
            IProp[] containerProps = {new Equipment(equipmentScheme, Array.Empty<ITacticalActScheme>())};

            FixedPropChest container = new FixedPropChest(containerProps);

            // трансферная машина
            PropTransferMachine transferMachine = new PropTransferMachine(inventory, container.Content);

            // ACT
            using var monitorInventory = transferMachine.Inventory.Monitor();
            using var monitorContainer = transferMachine.Container.Monitor();
            IProp transferResource = containerProps.First();
            transferMachine.TransferProp(transferResource,
                PropTransferMachineStore.Container,
                PropTransferMachineStore.Inventory);

            // ASSERT
            monitorInventory.Should().Raise(nameof(PropTransferStore.Added));
            monitorContainer.Should().Raise(nameof(PropTransferStore.Removed));
        }
    }
}