using FluentAssertions;
using LightInject;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Commands;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture()]
    public class UseSelfCommandTests: CommandTestBase
    {
        /// <summary>
        /// Тест проверяет, что можно использовать предмет, если есть информация об использовании.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = _container.GetInstance<UseSelfCommand>();



            // ACT
            var canExecute = command.CanExecute();


            // ASSERT
            canExecute.Should().Be(true);
        }

        protected override void RegisterSpecificServices(IMap testMap, Mock<IPlayerState> playerStateMock)
        {
            var propScheme = new PropScheme
            {
                Use = new TestPropUseSubScheme {
                    
                }
            };
            var usableResource = new Resource(propScheme, 1);

            var equipmentViewModelMock = new Mock<IPropItemViewModel>();
            equipmentViewModelMock.SetupGet(x => x.Prop).Returns(usableResource);
            var equipmentViewModel = equipmentViewModelMock.Object;

            var inventoryStateMock = new Mock<IInventoryState>();
            inventoryStateMock.SetupProperty(x => x.SelectedProp, equipmentViewModel);
            var inventoryState = inventoryStateMock.Object;

            _container.Register(factory => inventoryState, new PerContainerLifetime());

            _container.Register<UseSelfCommand>(new PerContainerLifetime());
        }
    }
}