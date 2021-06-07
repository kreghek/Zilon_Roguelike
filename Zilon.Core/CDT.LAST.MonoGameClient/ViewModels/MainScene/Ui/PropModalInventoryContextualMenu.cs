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
using Zilon.Core.PersonModules;
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

    public sealed class PropModalInventoryContextualMenu
    {
        private const int MENU_MARGIN = 2;
        private const int MENU_WIDTH = 128;
        private readonly IEquipmentModule _equipmentModule;

        private readonly TextButton[] _menuItemButtons;
        private readonly Point _position;
        private readonly IProp _prop;
        private readonly IServiceProvider _serviceProvider;
        private readonly Point _size;
        private readonly IUiContentStorage _uiContentStorage;

        public PropModalInventoryContextualMenu(Point position, IProp prop, IEquipmentModule equipmentModule,
            IUiContentStorage uiContentStorage,
            IServiceProvider serviceProvider)
        {
            _position = new Point(position.X - MENU_MARGIN, position.Y - MENU_MARGIN);
            _prop = prop;
            _equipmentModule = equipmentModule;
            _uiContentStorage = uiContentStorage;
            _serviceProvider = serviceProvider;

            var inventoryState = _serviceProvider.GetRequiredService<IInventoryState>();
            inventoryState.SelectedProp = new PropViewModel(_prop);
            _menuItemButtons = InitItems(prop);

            _size = new Point(MENU_WIDTH + MENU_MARGIN * 2, _menuItemButtons.Length * 32 + MENU_MARGIN * 2);
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

        private TextButton[] InitItems(IProp prop)
        {
            var list = new List<TextButton>();

            var useCommand = _serviceProvider.GetRequiredService<UseSelfCommand>();

            var commandPool = _serviceProvider.GetRequiredService<ICommandPool>();

            //TODO Localize

            switch (prop)
            {
                case Equipment equipment:
                    for (var slotIndex = 0; slotIndex < _equipmentModule.Slots.Length; slotIndex++)
                    {
                        var slot = _equipmentModule.Slots[slotIndex];
                        var equipCommand = _serviceProvider.GetRequiredService<EquipCommand>();
                        equipCommand.SlotIndex = slotIndex;
                        if (equipCommand.CanExecute().IsSuccess)
                        {
                            var equipButton = new TextButton($"Equip in {slot.Types}",
                                _uiContentStorage.GetButtonTexture(),
                                _uiContentStorage.GetButtonFont(),
                                new Rectangle(MENU_MARGIN + _position.X, MENU_MARGIN + _position.Y + slotIndex * 32,
                                    MENU_WIDTH - MENU_MARGIN * 2, 32));
                            equipButton.OnClick += (s, e) =>
                            {
                                commandPool.Push(equipCommand);
                                IsClosed = true;
                            };
                            list.Add(equipButton);
                        }
                    }

                    break;

                case Resource resource:
                    //TODO Different words to different resources.
                    if (useCommand.CanExecute().IsSuccess)
                    {
                        var useButton = new TextButton("Use", _uiContentStorage.GetButtonTexture(),
                            _uiContentStorage.GetButtonFont(),
                            new Rectangle(MENU_MARGIN + _position.X, MENU_MARGIN + _position.Y,
                                MENU_WIDTH - MENU_MARGIN * 2, 32));
                        useButton.OnClick += (s, e) =>
                        {
                            commandPool.Push(useCommand);
                            IsClosed = true;
                        };
                        list.Add(useButton);
                    }

                    break;
            }

            return list.ToArray();
        }
    }
}