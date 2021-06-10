using System;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Props;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal sealed class ContainerModalContainerContextualMenu
    {
        private const int MENU_MARGIN = 5;
        private const int MENU_WIDTH = 128;
        private const int MENU_ITEM_HEIGHT = 16;
        private readonly IPropStore _containerStore;
        private readonly IPropStore _inventoryStore;

        private readonly TextButton[] _menuItemButtons;
        private readonly Point _position;
        private readonly IServiceProvider _serviceProvider;

        private readonly Point _size;
        private readonly IUiContentStorage _uiContentStorage;

        public ContainerModalContainerContextualMenu(
            Point position,
            IProp prop,
            IPropStore inventoryStore,
            IPropStore containerStore,
            IUiContentStorage uiContentStorage,
            IServiceProvider serviceProvider)
        {
            _position = position;
            _inventoryStore = inventoryStore;
            _containerStore = containerStore;
            _uiContentStorage = uiContentStorage;
            _serviceProvider = serviceProvider;

            _menuItemButtons = InitItems(prop);

            var itemsHeight = _menuItemButtons.Length * MENU_ITEM_HEIGHT;
            _size = new Point(
                MENU_WIDTH + (MENU_MARGIN * 2),
                itemsHeight + (MENU_MARGIN * 2)
            );
        }

        public bool IsClosed { get; private set; }

        public bool IsCommandUsed { get; private set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawBorder(spriteBatch);

            foreach (var button in _menuItemButtons)
            {
                button.Draw(spriteBatch);
            }
        }

        public void Update()
        {
            foreach (var button in _menuItemButtons)
            {
                button.Update();
            }

            // Close menu if mouse is not on menu.

            var mouseState = Mouse.GetState();
            var mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            var menuRect = new Rectangle(_position, _size);
            if (!mouseRect.Intersects(menuRect))
            {
                CloseMenu();
            }
        }

        private void CloseMenu()
        {
            IsClosed = true;
        }

        private void DrawBorder(SpriteBatch spriteBatch)
        {
            // edges

            const int EDGE_SIZE = 5;
            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(_position, new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(0, 0, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + _size.X - EDGE_SIZE, _position.Y),
                    new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(7, 0, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X, _position.Y + _size.Y - EDGE_SIZE),
                    new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(0, 7, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + _size.X - EDGE_SIZE, _position.Y + _size.Y - EDGE_SIZE),
                    new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(7, 7, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            // sides

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + EDGE_SIZE, _position.Y), new Point(_size.X - (EDGE_SIZE * 2), 6)),
                new Rectangle(EDGE_SIZE, 0, 2, 6),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + EDGE_SIZE, _position.Y + _size.Y - EDGE_SIZE),
                    new Point(_size.X - (EDGE_SIZE * 2), 6)),
                new Rectangle(EDGE_SIZE, 7, 2, 6),
                Color.White);
        }

        private TextButton[] InitItems(IProp prop)
        {
            var takeMenuButton = new TextButton("Take", _uiContentStorage.GetMenuItemTexture(),
                _uiContentStorage.GetMenuItemFont(),
                new Rectangle(
                    MENU_MARGIN + _position.X,
                    MENU_MARGIN + _position.Y + (0 * MENU_ITEM_HEIGHT),
                    MENU_WIDTH,
                    MENU_ITEM_HEIGHT));

            takeMenuButton.OnClick += (s, e) =>
            {
                var transferCommand = _serviceProvider.GetRequiredService<PropTransferCommand>();
                var commandPool = _serviceProvider.GetRequiredService<ICommandPool>();

                var transferMachine = new PropTransferMachine(_inventoryStore, _containerStore);
                transferMachine.TransferProp(prop, PropTransferMachineStore.Container,
                    PropTransferMachineStore.Inventory);
                transferCommand.TransferMachine = transferMachine;

                commandPool.Push(transferCommand);
                IsCommandUsed = true;
                CloseMenu();
            };

            return new[] { takeMenuButton };
        }
    }

    internal sealed class ContainerModalInventoryContextualMenu
    {
        private const int MENU_MARGIN = 5;
        private const int MENU_WIDTH = 128;
        private const int MENU_ITEM_HEIGHT = 16;
        private readonly IPropStore _containerStore;
        private readonly IPropStore _inventoryStore;

        private readonly TextButton[] _menuItemButtons;
        private readonly Point _position;
        private readonly IServiceProvider _serviceProvider;

        private readonly Point _size;
        private readonly IUiContentStorage _uiContentStorage;

        public ContainerModalInventoryContextualMenu(
            Point position,
            IProp prop,
            IPropStore inventoryStore,
            IPropStore containerStore,
            IUiContentStorage uiContentStorage,
            IServiceProvider serviceProvider)
        {
            _position = position;
            _inventoryStore = inventoryStore;
            _containerStore = containerStore;
            _uiContentStorage = uiContentStorage;
            _serviceProvider = serviceProvider;

            _menuItemButtons = InitItems(prop);

            var itemsHeight = _menuItemButtons.Length * MENU_ITEM_HEIGHT;
            _size = new Point(
                MENU_WIDTH + (MENU_MARGIN * 2),
                itemsHeight + (MENU_MARGIN * 2)
            );
        }

        public bool IsClosed { get; private set; }

        public bool IsCommandUsed { get; private set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawBorder(spriteBatch);

            foreach (var button in _menuItemButtons)
            {
                button.Draw(spriteBatch);
            }
        }

        public void Update()
        {
            foreach (var button in _menuItemButtons)
            {
                button.Update();
            }

            // Close menu if mouse is not on menu.

            var mouseState = Mouse.GetState();
            var mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            var menuRect = new Rectangle(_position, _size);
            if (!mouseRect.Intersects(menuRect))
            {
                CloseMenu();
            }
        }

        private void CloseMenu()
        {
            IsClosed = true;
        }

        private void DrawBorder(SpriteBatch spriteBatch)
        {
            // edges

            const int EDGE_SIZE = 5;
            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(_position, new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(0, 0, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + _size.X - EDGE_SIZE, _position.Y),
                    new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(7, 0, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X, _position.Y + _size.Y - EDGE_SIZE),
                    new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(0, 7, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + _size.X - EDGE_SIZE, _position.Y + _size.Y - EDGE_SIZE),
                    new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(7, 7, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            // sides

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + EDGE_SIZE, _position.Y), new Point(_size.X - (EDGE_SIZE * 2), 6)),
                new Rectangle(EDGE_SIZE, 0, 2, 6),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + EDGE_SIZE, _position.Y + _size.Y - EDGE_SIZE),
                    new Point(_size.X - (EDGE_SIZE * 2), 6)),
                new Rectangle(EDGE_SIZE, 7, 2, 6),
                Color.White);
        }

        private TextButton[] InitItems(IProp prop)
        {
            var takeMenuButton = new TextButton("Store", _uiContentStorage.GetMenuItemTexture(),
                _uiContentStorage.GetMenuItemFont(),
                new Rectangle(
                    MENU_MARGIN + _position.X,
                    MENU_MARGIN + _position.Y + (0 * MENU_ITEM_HEIGHT),
                    MENU_WIDTH,
                    MENU_ITEM_HEIGHT));

            takeMenuButton.OnClick += (s, e) =>
            {
                var transferCommand = _serviceProvider.GetRequiredService<PropTransferCommand>();
                var commandPool = _serviceProvider.GetRequiredService<ICommandPool>();

                var transferMachine = new PropTransferMachine(_inventoryStore, _containerStore);
                transferMachine.TransferProp(prop, PropTransferMachineStore.Inventory,
                    PropTransferMachineStore.Container);
                transferCommand.TransferMachine = transferMachine;

                commandPool.Push(transferCommand);
                IsCommandUsed = true;
                CloseMenu();
            };

            return new[] { takeMenuButton };
        }
    }
}