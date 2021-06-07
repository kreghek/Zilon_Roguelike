using System;
using System.Collections.Generic;

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
    public class PropViewModel : IPropItemViewModel
    {
        public PropViewModel(IProp prop)
        {
            Prop = prop ?? throw new ArgumentNullException(nameof(prop));
        }

        public IProp Prop { get; }
    }

    public sealed class PropModalSubmenu
    {
        private const int MENU_MARGIN = 5;
        private readonly Point _position;
        private readonly Point _size;
        private readonly IProp _prop;
        private readonly IUiContentStorage _uiContentStorage;
        private readonly IServiceProvider _serviceProvider;
        private readonly TextButton[] _menuItemButtons;

        public PropModalSubmenu(Point position, IProp prop, IUiContentStorage uiContentStorage, IServiceProvider serviceProvider)
        {
            _position = new Point(position.X - MENU_MARGIN, position.Y - MENU_MARGIN);
            _size = new Point(100, 64);
            _prop = prop;
            _uiContentStorage = uiContentStorage;
            _serviceProvider = serviceProvider;
            _menuItemButtons = InitItems(prop);

            var inventoryState = _serviceProvider.GetRequiredService<IInventoryState>();
            inventoryState.SelectedProp = new PropViewModel(_prop);
        }

        private TextButton[] InitItems(IProp prop)
        {
            var list = new List<TextButton>();

            var equipCommand = _serviceProvider.GetRequiredService<EquipCommand>();
            equipCommand.SlotIndex = 0;

            var useCommand = _serviceProvider.GetRequiredService<EquipCommand>();

            //TODO Localize

            switch (prop)
            {
                case Equipment equipment:

                    if (equipCommand.CanExecute().IsSuccess)
                    {
                        var equipButton = new TextButton("Equip", _uiContentStorage.GetButtonTexture(), _uiContentStorage.GetButtonFont(),
                            new Rectangle(MENU_MARGIN, MENU_MARGIN, _size.X - MENU_MARGIN * 2, 32));
                        equipButton.OnClick += (s, e) =>
                        {
                            var equipCommand = _serviceProvider.GetRequiredService<EquipCommand>();
                            var commandPool = _serviceProvider.GetRequiredService<ICommandPool>();

                            commandPool.Push(equipCommand);
                        };
                        list.Add(equipButton);
                    }
                    break;

                case Resource resource:
                    //TODO Different words to different resources.
                    var useButton = new TextButton("Use", _uiContentStorage.GetButtonTexture(), _uiContentStorage.GetButtonFont(),
                        new Rectangle(MENU_MARGIN, MENU_MARGIN, _size.X - MENU_MARGIN * 2, 32));
                    list.Add(useButton);
                    break;
            }

            return list.ToArray();
        }

        public bool IsClosed { get; private set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), new Rectangle(_position, _size),
                Color.White);

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
                IsClosed = true;
            }
        }

        private void EquipButton_OnClick(object? sender, EventArgs e)
        {
            IsClosed = true;
        }
    }
}