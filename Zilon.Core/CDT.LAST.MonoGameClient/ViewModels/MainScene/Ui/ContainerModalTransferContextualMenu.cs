using System;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Props;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal sealed class ContainerModalTransferContextualMenu: PropModalInventoryContextualMenuBase
    {
        private const int MENU_MARGIN = 5;
        private const int MENU_WIDTH = 128;
        private const int MENU_ITEM_HEIGHT = 16;
        private readonly IPropStore _containerStore;
        private readonly IPropStore _inventoryStore;

        private readonly IServiceProvider _serviceProvider;
        private readonly PropTransferMachineStore _sourceStore;
        private readonly PropTransferMachineStore _targetStore;
        private readonly string _menuTitle;

        public ContainerModalTransferContextualMenu(
            Point position,
            IProp prop,
            IPropStore inventoryStore,
            IPropStore containerStore,
            IUiContentStorage uiContentStorage,
            IServiceProvider serviceProvider,
            PropTransferMachineStore sourceStore,
            PropTransferMachineStore targetStore,
            string menuTitle): base(position, prop, uiContentStorage)
        {
            _inventoryStore = inventoryStore;
            _containerStore = containerStore;
            _serviceProvider = serviceProvider;
            _sourceStore = sourceStore;
            _targetStore = targetStore;
            _menuTitle = menuTitle;
        }

        

        protected override TextButton[] InitItems(IProp prop)
        {
            var menuButton = new TextButton(_menuTitle, _uiContentStorage.GetMenuItemTexture(),
                _uiContentStorage.GetMenuItemFont(),
                new Rectangle(
                    MENU_MARGIN + _position.X,
                    MENU_MARGIN + _position.Y + (0 * MENU_ITEM_HEIGHT),
                    MENU_WIDTH,
                    MENU_ITEM_HEIGHT));

            menuButton.OnClick += (s, e) =>
            {
                var transferCommand = _serviceProvider.GetRequiredService<PropTransferCommand>();
                var commandPool = _serviceProvider.GetRequiredService<ICommandPool>();

                var transferMachine = new PropTransferMachine(_inventoryStore, _containerStore);
                transferMachine.TransferProp(prop, _sourceStore, _targetStore);
                transferCommand.TransferMachine = transferMachine;

                commandPool.Push(transferCommand);
                IsCommandUsed = true;
                CloseMenu();
            };

            return new[] { menuButton };
        }
    }
}