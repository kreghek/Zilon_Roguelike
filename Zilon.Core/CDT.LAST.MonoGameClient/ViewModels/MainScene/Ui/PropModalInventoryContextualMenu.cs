using System;
using System.Collections.Generic;
using System.Diagnostics;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Resources;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Components;
using Zilon.Core.PersonModules;
using Zilon.Core.Props;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class PropModalInventoryContextualMenu
    {
        private const int MENU_MARGIN = 5;
        private const int MENU_WIDTH = 128;
        private const int MENU_ITEM_HEIGHT = 16;

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

        private void CloseMenu()
        {
            IsClosed = true;
            var inventoryState = _serviceProvider.GetRequiredService<IInventoryState>();
            inventoryState.SelectedProp = null;
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

        private static string GetInventoryMenuItemContent(IProp prop)
        {
            switch (prop.Scheme.Sid)
            {
                case "med-kit":
                    return UiResources.HealCommandButtonTitle;

                case "water-bottle":
                    return UiResources.DrinkCommandButtonTitle;

                case "packed-food":
                    return UiResources.EatCommandButtonTitle;

                default:
                    Debug.Fail("Every consumable must have definative command title.");
                    return UiResources.UseCommandButtonTitle;
            }
        }

        private static string GetSlotTitle(EquipmentSlotTypes types)
        {
            switch (types)
            {
                case EquipmentSlotTypes.Hand:
                    return UiResources.SlotHand;

                case EquipmentSlotTypes.Head:
                    return UiResources.SlotHead;

                case EquipmentSlotTypes.Body:
                    return UiResources.SlotBody;

                case EquipmentSlotTypes.Aux:
                    return UiResources.SlotAux;

                default:
                    Debug.Fail("All slot types must have name.");
                    return "<Unknown>";
            }
        }

        private TextButton[] InitItems(IProp prop)
        {
            var list = new List<TextButton>();

            var useCommand = _serviceProvider.GetRequiredService<UseSelfCommand>();

            var commandPool = _serviceProvider.GetRequiredService<ICommandPool>();

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
                            var slotTitle = GetSlotTitle(slot.Types);
                            var equipButtonTitle =
                                string.Format(UiResources.EquipInSlotTemplateCommandButton, slotTitle);
                            var equipButton = new TextButton(equipButtonTitle,
                                _uiContentStorage.GetMenuItemTexture(),
                                _uiContentStorage.GetMenuItemFont(),
                                new Rectangle(
                                    MENU_MARGIN + _position.X,
                                    MENU_MARGIN + _position.Y + (list.Count * MENU_ITEM_HEIGHT),
                                    MENU_WIDTH,
                                    MENU_ITEM_HEIGHT));
                            equipButton.OnClick += (s, e) =>
                            {
                                commandPool.Push(equipCommand);
                                CloseMenu();
                                IsCommandUsed = true;
                            };
                            list.Add(equipButton);
                        }
                    }

                    break;

                case Resource resource:
                    if (useCommand.CanExecute().IsSuccess)
                    {
                        var localizedCommandTitle = GetInventoryMenuItemContent(_prop);
                        var useButton = new TextButton(localizedCommandTitle,
                            _uiContentStorage.GetMenuItemTexture(),
                            _uiContentStorage.GetMenuItemFont(),
                            new Rectangle(
                                MENU_MARGIN + _position.X,
                                MENU_MARGIN + _position.Y,
                                MENU_WIDTH,
                                MENU_ITEM_HEIGHT));
                        useButton.OnClick += (s, e) =>
                        {
                            commandPool.Push(useCommand);
                            CloseMenu();
                            IsCommandUsed = true;
                        };
                        list.Add(useButton);
                    }

                    break;
            }

            return list.ToArray();
        }

        private record IventoryMenuItemContent
        {
            public IventoryMenuItemContent(SoundEffect consumeSoundEffect, string title)
            {
                ConsumeSoundEffect = consumeSoundEffect ?? throw new ArgumentNullException(nameof(consumeSoundEffect));
                Title = title ?? throw new ArgumentNullException(nameof(title));
            }

            public SoundEffect ConsumeSoundEffect { get; }
            public string Title { get; }
        }
    }
}