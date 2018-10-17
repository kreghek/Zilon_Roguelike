using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Client
{
    [TestFixture]
    public class PropTransferMachineTests
    {
        [Test]
        public void TransferPropTest()
        {
            // ARRANGE

            var resourceScheme = new PropScheme();

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
    }
}