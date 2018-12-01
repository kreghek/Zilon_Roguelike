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
                transferMachine.Container,
                transferMachine.Inventory);



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

            var resourceScheme = new TestPropScheme {
                Equip = new TestPropEquipSubScheme()
            };

            // Инвентарь
            var inventory = new Inventory();

            // контейнер
            var containerProps = new IProp[] {
                new Equipment(resourceScheme, new ITacticalActScheme[0])
            };
            var nodeMock = new Mock<IMapNode>();
            var node = nodeMock.Object;
            var container = new FixedPropChest(node, containerProps);

            // трансферная машина
            var transferMachine = new PropTransferMachine(inventory, container.Content);



            // ACT
            var transferResource = containerProps.First();
            transferMachine.TransferProp(transferResource,
                transferMachine.Container,
                transferMachine.Inventory);



            // ASSERT
            transferMachine.Inventory.PropAdded[0].Should().BeOfType<Equipment>();
            transferMachine.Container.PropRemoved[0].Should().BeOfType<Equipment>();
        }
    }
}